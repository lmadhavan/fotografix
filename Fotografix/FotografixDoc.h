// FotografixDoc.h : interface of the CFotografixDoc class
//


#pragma once

#include "FGXImage.h"
#include "FGXSelection.h"
#include "TransformTracker.h"
#include "Language.h"

#define MAKESIG(a, b, c, d) (LONG(a) | (LONG(b) << 8) | (LONG(c) << 16) | (LONG(d) << 24))

#include <vector>
using std::vector;

enum {
	UndoSelection,
	UndoLayer,
	UndoImage,
	UndoNewLayer,
	UndoDeleteLayer,
	UndoMoveLayer,
	UndoLayerSelection,
	UndoMoveSelection,
	UndoCanvasSize,
	UndoAdjustLayer,
	UndoModifyLayer,
	UndoAddLayerMask,
	UndoRevealAll,
	UndoFlipImage,
	UndoOrderLayer,
	UndoRasterize
};

struct FGXUndo {
	int type;
	int i1, i2, i3, i4, i5, i6;
	CString s1, s2;

	TCHAR text[32];

	FGXChannel channel;
	FGXLayer layer;
	FGXImage image;
	CPoint *pos;

	FGXUndo(int undoType, int i1 = 0, int i2 = 0, int i3 = 0, int i4 = 0, int i5 = 0, int i6 = 0) : type(undoType), i1(i1), i2(i2), i3(i3), i4(i4), i5(i5), i6(i6), pos(NULL) {
		text[0] = 0;
	}

	~FGXUndo() {
		if (pos != NULL) delete [] pos;
	}

	void SaveLayerPositions(const FGXImage &image) {
		pos = new CPoint[image.GetLayerCount()];
		for (int i = 0; i < image.GetLayerCount(); i++)
			pos[i] = image.GetLayer(i)->GetPosition().TopLeft();
	}

	void RestoreLayerPositions(FGXImage &image) const {
		for (int i = 0; i < image.GetLayerCount(); i++)
			image.GetLayer(i)->MoveTo(pos[i].x, pos[i].y);
	}
};

struct PsdLayer;

class CFotografixDoc : public CDocument
{
protected: // create from serialization only
	CFotografixDoc();
	DECLARE_DYNCREATE(CFotografixDoc)

public:
	static bool first;

// File format helpers
public:
	static int LoadImage_Bitmap(Bitmap &bitmap, FGXLayer &layer);
	static int LoadImage_Icon(HICON hIcon, FGXImage &image);
	static int LoadImage_Default(LPCTSTR path, FGXImage &image);

	static int SaveImage_Default(int type, LPCTSTR path, const FGXImage &image);

	static void PSD_ReadChannelData(CFile &file, FGXChannel &channel);
	static void PSD_ReadChannelDataRLE(CFile &file, FGXChannel &channel);
	static CString PSD_ReadString(CFile &file);
	static void PSD_ReadChannel(CFile &file, FGXChannel &channel, short Compression);
	static void ChannelRotate(FGXChannel **base, int count);
	static void PSD_ReadLayer(CFile &file, FGXLayer &layer, PsdLayer &lrec, short Mode, bool SeparateFlags);
	static int LoadImage_PSD(LPCTSTR path, FGXImage &image);

	static CString XCF_ReadString(CFile &file);
	static void XCF_ReadHierarchy(CFile &file, BYTE **channelData, int Compression);
	static bool XCF_ReadLayer(CFile &file, FGXLayer &layer, int Mode, int Compression);
	static int LoadImage_XCF(LPCTSTR path, FGXImage &image);

	static int LoadImage_PCX(LPCTSTR path, FGXImage &image);
	static int LoadImage_TGA(LPCTSTR path, FGXImage &image);
	static int LoadImage_RAW(LPCTSTR path, FGXImage &image);
	static int LoadImage_ICO(LPCTSTR path, FGXImage &image);
	static int LoadImage_EXE(LPCTSTR path, FGXImage &image);

	static int SaveImage_TGA(LPCTSTR path, const FGXImage &image);
	static int SaveImage_RAW(LPCTSTR path, const FGXImage &image);

	static int LoadImage_FGX(LPCTSTR path, FGXImage &image);
	static void FGX_WriteBlockHeader(CFile &file, LONG Signature, LONG BlockSize);
	static int SaveImage_FGX(LPCTSTR path, const FGXImage &image);

	// Overrides
public:
	virtual BOOL OnNewDocument();
	virtual BOOL OnOpenDocument(LPCTSTR lpszPathName);
	virtual BOOL OnSaveDocument(LPCTSTR lpszPathName);

// Implementation
public:
	virtual ~CFotografixDoc();
#ifdef _DEBUG
	virtual void AssertValid() const;
	virtual void Dump(CDumpContext& dc) const;
#endif

protected:
	virtual BOOL SaveModified();

// Generated message map functions
protected:
	DECLARE_MESSAGE_MAP()

private:
	void SetUndoMarker();

public:
	void ClearRedo();

	bool CanUndo() {
		return undoPos > -1;
	}

	bool CanRedo() {
		return undoPos < int(undo.size()) - 1;
	}

	const TCHAR *GetUndoText() {
		return CanUndo() ? undo[undoPos]->text : NULL;
	}

	const TCHAR *GetRedoText() {
		return CanRedo() ? undo[undoPos+1]->text : NULL;
	}

	bool CanPurgeUndo() {
		return undo.size() > 0;
	}

	int Undo();
	int Redo();
	void PurgeUndo();

	void AddUndo(int type, const TCHAR *text = LangItem(Selection));
	FGXChannel &GetUndoSelection(const TCHAR *text = LangItem(Selection));
	FGXLayer &GetUndoLayer(int index, const TCHAR *text = LangItem(Change));
	FGXImage &GetUndoImage(const TCHAR *text = LangItem(Change));
	FGXLayer &GetUndoLayerSelection(int index, const TCHAR *text = LangItem(Change));
	void AddUndoNewLayer(int index, const TCHAR *text = LangItem(NewLayer));
	void AddUndoDeleteLayer(int index, const TCHAR *text = LangItem(DeleteLayer));
	void SaveUndoCanvasSize(int anchor);
	void SaveUndoModifyLayer(int index);
	void SaveUndoMoveLayer(int index);
	void SaveUndoMoveSelection();
	void SaveUndoAddLayerMask(int index);
	void SaveUndoRevealAll();
	void SaveUndoFlipImage(int type, const TCHAR *text);
	void SaveUndoOrderLayer(int from, int to);
	void SaveUndoRasterize(int index);

	void SaveUndoLayer(int index, const TCHAR *text = LangItem(Change)) {
		FGXLayer *layer = image.GetLayer(index);
		layer->Clone(GetUndoLayer(index, text));
	}

public:
	void Redraw(bool sizeChanged = false);
	void Redraw(const CRect &rect);

	void RedrawSelection() {
		Redraw(selection.GetPosition());
	}

	void RedrawLayer(int layer) {
		Redraw(image.GetLayer(layer)->GetPosition());
	}

	void RedrawLayer(int layer, const CRect &rect) {
		Redraw(rect & image.GetLayer(layer)->GetPosition());
	}

	void RedrawLayerSelection(int layer) {
		RedrawLayer(layer, selection.GetPosition());
	}

	void CleanSelection();
	void PrepareSelection();

public:
	FGXImage image;
	FGXSelection selection;
	FGXBitmap bitmap;
	CRect bounds;

	int selChannel;
	int channelMask;

	// Transform information
	bool tActive;
	CTransformTracker tTracker;

	bool hasPath;
	bool dirty;

private:
	vector<FGXUndo *> undo;
	int undoPos;
};
