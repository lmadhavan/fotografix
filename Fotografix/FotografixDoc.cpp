// FotografixDoc.cpp : implementation of the CFotografixDoc class
//

#include "stdafx.h"
#include "Fotografix.h"

#include "FotografixDoc.h"
#include "NewDialog.h"
#include "RawDialog.h"
#include "JPEGDialog.h"

#include "MainFrm.h"
#include "FotografixView.h"

#include "FGX.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif

// CFotografixDoc

bool CFotografixDoc::first = true;

IMPLEMENT_DYNCREATE(CFotografixDoc, CDocument)

BEGIN_MESSAGE_MAP(CFotografixDoc, CDocument)
END_MESSAGE_MAP()


// CFotografixDoc construction/destruction

CFotografixDoc::CFotografixDoc()
{
	undoPos = -1;

	selChannel = 0;
	channelMask = ChannelAll;

	tActive = false;
	tTracker.m_nHandleSize = 6;
	tTracker.m_nStyle = CRectTracker::hatchedBorder | CRectTracker::resizeOutside;
	tTracker.m_sizeMin = CSize(1, 1);

	hasPath = false;
	dirty = false;
}

CFotografixDoc::~CFotografixDoc()
{
	PurgeUndo();
}

BOOL CFotografixDoc::OnNewDocument()
{
	if (first == true) {
		first = false;
		return false;
	}

	if (CDocument::OnNewDocument() == false)
		return false;

	CNewDialog dlg;
	if (dlg.DoModal() == IDOK) {
		image.Initialize(dlg.w, dlg.h);
		image.SetResolution(dlg.res);
		image.SetUnit(dlg.unit);

		FGXLayer *layer = image.AddLayer();
		layer->SetName(LangItem(Layer) + TEXT(" 1"));

		switch (dlg.fill) {
			case 0:
				layer->SetPosition(CRect(0, 0, dlg.w, dlg.h));
				layer->Fill(FGXColor(255, 255, 255, 255));
				break;

			case 1:
				layer->SetPosition(CRect(0, 0, dlg.w, dlg.h));
				layer->Fill(FGXColor(globals.bgColor.GetColor(), 255));
				break;
		}

		return true;
	}

	return false;
}

int CFotografixDoc::LoadImage_Bitmap(Bitmap &bitmap, FGXLayer &layer) {
	BitmapData data;

	int status = bitmap.LockBits(&Rect(0, 0, bitmap.GetWidth(), bitmap.GetHeight()), ImageLockMode::ImageLockModeRead, PixelFormat32bppARGB, &data);

	if (status == Status::Ok) {
		layer.SetPosition(CRect(0, 0, bitmap.GetWidth(), bitmap.GetHeight()));
		layer.LoadFromMemory(data.Scan0, 4);

		bitmap.UnlockBits(&data);
	}

	return status;
}

int CFotografixDoc::LoadImage_Icon(HICON hIcon, FGXImage &image) {
	Bitmap bitmap(hIcon);

	int status = bitmap.GetLastStatus();
	
	if (status == Status::Ok) {
		FGXLayer *layer = image.AddLayer();
		if ((status = LoadImage_Bitmap(bitmap, *layer)) == Status::Ok)
			image.Initialize(layer->GetPosition().Width(), layer->GetPosition().Height());
	}

	return status;
}

int CFotografixDoc::LoadImage_Default(LPCTSTR path, FGXImage &image) {
	Bitmap bitmap(path);

	int status = bitmap.GetLastStatus();

	if (status == Status::Ok) {
		GUID frameGUID;
		if (bitmap.GetFrameDimensionsList(&frameGUID, 1) != Status::Ok)
			return Status::GenericError;

		unsigned numFrames = bitmap.GetFrameCount(&frameGUID);
		for (unsigned i = 0; i < numFrames; i++) {
			FGXLayer *layer = image.AddLayer();
			layer->SetVisible(false);
			if (bitmap.SelectActiveFrame(&frameGUID, i) == Status::Ok)
				status = LoadImage_Bitmap(bitmap, *layer);
		}

		if (numFrames > 0 && status == Status::Ok) {
			FGXLayer *layer = image.GetLayer(0);
			layer->SetVisible(true);
			image.Initialize(layer->GetPosition().Width(), layer->GetPosition().Height());
			image.SetResolution(bitmap.GetHorizontalResolution());
		}
	}

	return status;
}

BOOL CFotografixDoc::OnOpenDocument(LPCTSTR lpszPathName)
{
	LPCTSTR temp = _tcsrchr(lpszPathName, '.');
	CString ext;

	if (temp != NULL) {
		ext = temp + 1;
		ext.MakeLower();
	}

	int status;

	if (globals.openExtract == true)
		status = LoadImage_EXE(lpszPathName, image);
	else if (ext == TEXT("jpg") || ext == TEXT("jpeg") || ext == TEXT("jpe") || ext == TEXT("jfif"))
		status = LoadImage_Default(lpszPathName, image), hasPath = true;
	else if (ext == TEXT("bmp"))
		status = LoadImage_Default(lpszPathName, image), hasPath = true;
	else if (ext == TEXT("png"))
		status = LoadImage_Default(lpszPathName, image), hasPath = true;
	else if (ext == TEXT("gif"))
		status = LoadImage_Default(lpszPathName, image), hasPath = true;
	else if (ext == TEXT("tif") || ext == TEXT("tiff"))
		status = LoadImage_Default(lpszPathName, image), hasPath = true;
	else if (ext == TEXT("fgx"))
		status = LoadImage_FGX(lpszPathName, image), hasPath = true;
	else if (ext == TEXT("psd"))
		status = LoadImage_PSD(lpszPathName, image);
	else if (ext == TEXT("xcf"))
		status = LoadImage_XCF(lpszPathName, image);
	else if (ext == TEXT("tga"))
		status = LoadImage_TGA(lpszPathName, image), hasPath = true;
	else if (ext == TEXT("pcx"))
		status = LoadImage_PCX(lpszPathName, image);
	else if (ext == TEXT("ico") || ext == TEXT("cur"))
		status = LoadImage_ICO(lpszPathName, image);
	else if (ext == TEXT("raw"))
		status = LoadImage_RAW(lpszPathName, image);
	else
		status = LoadImage_Default(lpszPathName, image);

	if (image.GetLayerCount() == 0)
		status = Status::UnknownImageFormat;

	switch (status) {
	case Status::Ok:
		return true;

	case Status::NotImplemented:
		AfxGetMainWnd()->MessageBox(LangMessageParam(WarnExtraData, lpszPathName), NULL, MB_ICONWARNING | MB_OK);
		return true;

	case Status::WrongState:
		AfxGetMainWnd()->MessageBox(LangMessageParam(WarnLostData, lpszPathName), NULL, MB_ICONWARNING | MB_OK);
		return true;

	case Status::FileNotFound:
	case Status::AccessDenied:
		AfxGetMainWnd()->MessageBox(LangMessageParam(ErrorFileRead, lpszPathName), NULL, MB_ICONSTOP | MB_OK);
		return false;

	case Status::Aborted:
		return false;

	default:
		AfxGetMainWnd()->MessageBox(LangMessageParam(ErrorFileFormat, lpszPathName), NULL, MB_ICONSTOP | MB_OK);
		return false;
	}
}

// MSDN: Helper function to retrieve CLSID for given format
int GetEncoderClsid(const WCHAR* format, CLSID* pClsid)
{
   UINT  num = 0;          // number of image encoders
   UINT  size = 0;         // size of the image encoder array in bytes

   Gdiplus::ImageCodecInfo* pImageCodecInfo = NULL;

   Gdiplus::GetImageEncodersSize(&num, &size);
   if(size == 0)

      return -1;  // Failure

   pImageCodecInfo = (Gdiplus::ImageCodecInfo*)(malloc(size));
   if(pImageCodecInfo == NULL)
      return -1;  // Failure

   Gdiplus::GetImageEncoders(num, size, pImageCodecInfo);

   for(UINT j = 0; j < num; ++j)
   {
      if( wcscmp(pImageCodecInfo[j].MimeType, format) == 0 )
      {
         *pClsid = pImageCodecInfo[j].Clsid;
         free(pImageCodecInfo);
         return j;  // Success
      }    
   }

   free(pImageCodecInfo);
   return -1;  // Failure
}

enum {
	EncoderJPEG,
	EncoderBMP,
	EncoderPNG,
	EncoderGIF,
	EncoderTIFF
};

static WCHAR *mimeType[] = {
	TEXT("image/jpeg"),
	TEXT("image/bmp"),
	TEXT("image/png"),
	TEXT("image/gif"),
	TEXT("image/tiff")
};

static PixelFormat pixelType[] = {
	PixelFormat24bppRGB,
	PixelFormat24bppRGB,
	PixelFormat32bppARGB,
	PixelFormat32bppARGB /*PixelFormat8bppIndexed*/,
	PixelFormat32bppARGB
};

//void CreateIndexedImage(DWORD *src, BYTE *dest, unsigned count, ColorPalette *palette) {
//	for (unsigned i = 0; i < count; i++)
//		dest[i] = i < count / 2 ? 0 : 1;
//	palette->Count = 2;
//	palette->Entries[0] = 0x00ffff00;
//	palette->Entries[1] = 0xff0000ff;
//}

int CFotografixDoc::SaveImage_Default(int type, LPCTSTR path, const FGXImage &image) {
	CLSID clsid;
	if (GetEncoderClsid(mimeType[type], &clsid) == -1)
		return Status::GenericError;

	EncoderParameters params = { 0 };
	UINT quality;

	if (type == EncoderJPEG) {
		CJPEGDialog dialog;

		if (dialog.DoModal() == IDCANCEL)
			return Status::Aborted;

		quality = dialog.quality;

		params.Count = 1;
		params.Parameter[0].Guid = Gdiplus::EncoderQuality;
		params.Parameter[0].Type = Gdiplus::EncoderParameterValueTypeLong;
		params.Parameter[0].NumberOfValues = 1;
		params.Parameter[0].Value = &quality;
	}

	int w = image.GetWidth(),
		h = image.GetHeight();

	bool transparent = (type == EncoderPNG || type == EncoderGIF || type == EncoderTIFF);

	FGXLayer layer(CRect(0, 0, w, h));
	if (transparent == false) layer.Fill(FGXColor(globals.bgColor.GetColor(), 255));
	image.Render(layer, CRect(0, 0, w, h), transparent);

	DWORD *p = new DWORD[w * h];
	layer.SaveToMemory(p, 4);

	//BYTE *pp = NULL;
	//char palBuffer[sizeof(ColorPalette) + 255 * sizeof(ARGB)];
	//ColorPalette *palette = reinterpret_cast<ColorPalette *>(palBuffer);
	//if (type == EncoderGIF) {
	//	// Create palette based on image
	//	pp = new BYTE[w * h];
	//	palette->Flags = Gdiplus::PaletteFlags::PaletteFlagsHasAlpha;
	//	palette->Count = 256;
	//	CreateIndexedImage(p, pp, w * h, palette);
	//}

	Bitmap bitmap(w, h, pixelType[type]);

	BitmapData data;
	data.Width = w;
	data.Height = h;
	//if (type == EncoderGIF) {
	//	data.Stride = w;
	//	data.PixelFormat = PixelFormat8bppIndexed;
	//	data.Scan0 = pp;
	//} else {
		data.Stride = w * 4;
		data.PixelFormat = PixelFormat32bppARGB;
		data.Scan0 = p;
	//}
	data.Reserved = NULL;

	Rect rect(0, 0, w, h);

	bitmap.LockBits(&rect, ImageLockMode::ImageLockModeWrite | ImageLockMode::ImageLockModeUserInputBuf, /*(type == EncoderGIF ? PixelFormat8bppIndexed : PixelFormat32bppARGB)*/ PixelFormat32bppARGB, &data);
	bitmap.UnlockBits(&data);

	//if (pp != NULL) {
	//	bitmap.SetPalette(palette);
	//	delete pp;
	//}
	delete p;

	int res;
	switch (image.GetUnit()) {
	case 0:
		res = 72;
		break;

	case 1:
		res = image.GetResolution();
		break;

	case 2:
		res = image.GetResolution() / 2.54f;
		break;
	}
	bitmap.SetResolution(res, res);

	return params.Count > 0 ? bitmap.Save(path, &clsid, &params) : bitmap.Save(path, &clsid);
}

BOOL CFotografixDoc::OnSaveDocument(LPCTSTR lpszPathName)
{
	LPCTSTR temp = _tcsrchr(lpszPathName, '.');
	CString ext;

	if (temp != NULL) {
		ext = temp + 1;
		ext.MakeLower();
	}

	int status = Status::Ok;

	if (ext == TEXT("jpg") || ext == TEXT("jpeg") || ext == TEXT("jpe") || ext == TEXT("jfif"))
		status = SaveImage_Default(EncoderJPEG, lpszPathName, image);
	else if (ext == TEXT("bmp"))
		status = SaveImage_Default(EncoderBMP, lpszPathName, image);
	else if (ext == TEXT("png"))
		status = SaveImage_Default(EncoderPNG, lpszPathName, image);
	else if (ext == TEXT("gif"))
		status = SaveImage_Default(EncoderGIF, lpszPathName, image);
	else if (ext == TEXT("tif") || ext == TEXT("tiff"))
		status = SaveImage_Default(EncoderTIFF, lpszPathName, image);
	else if (ext == TEXT("fgx"))
		status = SaveImage_FGX(lpszPathName, image);
	else if (ext == TEXT("tga"))
		status = SaveImage_TGA(lpszPathName, image);
	else if (ext == TEXT("raw"))
		status = SaveImage_RAW(lpszPathName, image);

	switch (status) {
	case Status::Ok:
		return true;

	case Status::AccessDenied:
		AfxGetMainWnd()->MessageBox(LangMessageParam(ErrorFileWrite, lpszPathName), NULL, MB_ICONSTOP | MB_OK);
		return false;

	default:
		return false;
	}
}

// CFotografixDoc diagnostics

#ifdef _DEBUG
void CFotografixDoc::AssertValid() const
{
	CDocument::AssertValid();
}

void CFotografixDoc::Dump(CDumpContext& dc) const
{
	CDocument::Dump(dc);
}
#endif //_DEBUG


// CFotografixDoc commands

void CFotografixDoc::SetUndoMarker() {
	undoPos++;

	for (int i = undoPos; i < int(undo.size()); i++)
		delete undo[i];

	undo.resize(undoPos);
}

void CFotografixDoc::ClearRedo() {
	SetUndoMarker();
	undoPos--;
}

void CFotografixDoc::AddUndo(int type, const TCHAR *text) {
	FGXUndo *u = new FGXUndo(UndoSelection);
	_tcscpy(u->text, text);

	SetUndoMarker();
	undo.push_back(u);
}

FGXChannel &CFotografixDoc::GetUndoSelection(const TCHAR *text) {
	FGXUndo *u = new FGXUndo(UndoSelection);
	_tcscpy(u->text, text);

	SetUndoMarker();
	undo.push_back(u);

	return u->channel;
}

FGXLayer &CFotografixDoc::GetUndoLayer(int index, const TCHAR *text) {
	FGXUndo *u = new FGXUndo(UndoLayer, index);
	_tcscpy(u->text, text);

	SetUndoMarker();
	undo.push_back(u);

	return u->layer;
}

FGXImage &CFotografixDoc::GetUndoImage(const TCHAR *text) {
	FGXUndo *u = new FGXUndo(UndoImage);
	_tcscpy(u->text, text);

	SetUndoMarker();
	undo.push_back(u);

	return u->image;
}

FGXLayer &CFotografixDoc::GetUndoLayerSelection(int index, const TCHAR *text) {
	const CRect &rect = selection.GetPosition();
	FGXUndo *u = new FGXUndo(UndoLayerSelection, index, rect.left, rect.top);
	_tcscpy(u->text, text);

	SetUndoMarker();
	undo.push_back(u);

	return u->layer;
}

void CFotografixDoc::AddUndoNewLayer(int index, const TCHAR *text) {
	FGXUndo *u = new FGXUndo(UndoNewLayer, index);
	_tcscpy(u->text, text);

	SetUndoMarker();
	undo.push_back(u);

	image.GetLayer(index)->Clone(u->layer);
}

void CFotografixDoc::AddUndoDeleteLayer(int index, const TCHAR *text) {
	FGXUndo *u = new FGXUndo(UndoDeleteLayer, index);
	_tcscpy(u->text, text);

	SetUndoMarker();
	undo.push_back(u);

	image.GetLayer(index)->Swap(u->layer);
}

void CFotografixDoc::SaveUndoCanvasSize(int anchor) {
	FGXUndo *u = new FGXUndo(UndoCanvasSize, bounds.right, bounds.bottom, anchor);
	_tcscpy(u->text, LangItem(CanvasSize));

	SetUndoMarker();
	undo.push_back(u);
}

void CFotografixDoc::SaveUndoModifyLayer(int index) {
	FGXLayer *layer = image.GetLayer(index);
	FGXUndo *u = new FGXUndo(UndoModifyLayer, index, layer->GetOpacity(), layer->GetMode(), layer->a1, layer->a2, layer->a3);
	u->s1 = layer->a4;
	u->s2 = layer->a5;
	_tcscpy(u->text, LangItem(LayerProperties));

	SetUndoMarker();
	undo.push_back(u);
}

void CFotografixDoc::SaveUndoMoveLayer(int index) {
	FGXLayer *layer = image.GetLayer(index);
	FGXUndo *u = new FGXUndo(UndoMoveLayer, index, layer->GetPosition().left, layer->GetPosition().top);
	_tcscpy(u->text, LangItem(Move));

	SetUndoMarker();
	undo.push_back(u);
}

void CFotografixDoc::SaveUndoMoveSelection() {
	FGXUndo *u = new FGXUndo(UndoMoveSelection, selection.GetPosition().left, selection.GetPosition().top);
	_tcscpy(u->text, LangItem(Move));

	SetUndoMarker();
	undo.push_back(u);
}

void CFotografixDoc::SaveUndoAddLayerMask(int index) {
	FGXUndo *u = new FGXUndo(UndoAddLayerMask, index);
	_tcscpy(u->text, LangItem(AddMask));

	SetUndoMarker();
	undo.push_back(u);
}

void CFotografixDoc::SaveUndoRevealAll() {
	FGXUndo *u = new FGXUndo(UndoRevealAll, image.GetWidth(), image.GetHeight());
	_tcscpy(u->text, LangItem(RevealAll));

	SetUndoMarker();
	undo.push_back(u);

	u->SaveLayerPositions(image);
}

void CFotografixDoc::SaveUndoFlipImage(int type, const TCHAR *text) {
	FGXUndo *u = new FGXUndo(UndoFlipImage, type);
	_tcscpy(u->text, text);

	SetUndoMarker();
	undo.push_back(u);
}

void CFotografixDoc::SaveUndoOrderLayer(int from, int to) {
	FGXUndo *u = new FGXUndo(UndoOrderLayer, from, to);
	_tcscpy(u->text, LangItem(LayerOrder));

	SetUndoMarker();
	undo.push_back(u);
}

void CFotografixDoc::SaveUndoRasterize(int index) {
	FGXUndo *u = new FGXUndo(UndoRasterize, index);
	_tcscpy(u->text, LangItem(RasterizeLayer));

	SetUndoMarker();
	undo.push_back(u);
}

int CFotografixDoc::Undo() {
	if (CanUndo()) {
		FGXUndo *u = undo[undoPos--];

		switch (u->type) {
			case UndoSelection:
				CleanSelection();
				u->channel.Swap(selection);
				PrepareSelection();
				break;

			case UndoLayer:
			case UndoLayerSelection:
				{
					CRect rect = u->layer.GetPosition();
					u->layer.Swap(*image.GetLayer(u->i1));
					rect |= u->layer.GetPosition();
					Redraw(rect);
				}
				if (u->type == UndoLayerSelection) {
					CRect rect = selection.GetPosition();
					CleanSelection();
					selection.MoveTo(u->i2, u->i3);
					PrepareSelection();
					u->i2 = rect.left, u->i3 = rect.top;
				}
				break;

			case UndoImage:
				u->image.Swap(image);
				Redraw(true);
				break;

			case UndoNewLayer:
				image.DeleteLayer(u->i1);
				Redraw(u->layer.GetPosition());
				break;

			case UndoDeleteLayer:
				image.InsertLayer(u->i1)->Swap(u->layer);
				RedrawLayer(u->i1);
				break;

			case UndoCanvasSize:
				image.ResizeCanvas(u->i1, u->i2, u->i3);
				u->i1 = bounds.right, u->i2 = bounds.bottom;
				Redraw(true);
				break;

			case UndoModifyLayer:
				{
					FGXLayer *layer = image.GetLayer(u->i1);

					BYTE o = layer->GetOpacity();
					layer->SetOpacity(u->i2);
					u->i2 = o;

					BYTE m = layer->GetMode();
					layer->SetMode(u->i3);
					u->i3 = m;

					swap(layer->a1, u->i4);
					swap(layer->a2, u->i5);
					swap(layer->a3, u->i6);
					swap(layer->a4, u->s1);
					swap(layer->a5, u->s2);

					if (layer->GetType() == LayerText) {
						layer->RenderText();
						Redraw();
					} else
						RedrawLayer(u->i1);
				}
				break;

			case UndoMoveLayer:
				{
					FGXLayer *layer = image.GetLayer(u->i1);
					CRect rect = layer->GetPosition();
					layer->MoveTo(u->i2, u->i3);
					u->i2 = rect.left, u->i3 = rect.top;
				}
				Redraw();
				break;

			case UndoMoveSelection:
				CleanSelection();
				{
					CRect rect = selection.GetPosition();
					selection.MoveTo(u->i1, u->i2);
					u->i1 = rect.left, u->i2 = rect.top;
				}
				PrepareSelection();
				break;

			case UndoAddLayerMask:
				image.GetLayer(u->i1)->DelMask(false);
				RedrawLayer(u->i1);
				break;

			case UndoRevealAll:
				image.Initialize(u->i1, u->i2);
				u->RestoreLayerPositions(image);
				Redraw(true);
				break;

			case UndoFlipImage:
				if (u->i1 == 0)
					image.Flip(true, false);
				else
					image.Flip(false, true);
				Redraw(true);
				break;

			case UndoOrderLayer:
				swap(u->i1, u->i2);
				image.MoveLayer(u->i1, u->i2);
				RedrawLayer(u->i2);
				break;

			case UndoRasterize:
				image.GetLayer(u->i1)->type = LayerText;
				RedrawLayer(u->i1);
				break;
		}

		return u->type;
	}

	return -1;
}

int CFotografixDoc::Redo() {
	if (CanRedo()) {
		FGXUndo *u = undo[++undoPos];

		switch (u->type) {
			case UndoSelection:
				CleanSelection();
				u->channel.Swap(selection);
				PrepareSelection();
				break;

			case UndoLayer:
			case UndoLayerSelection:
				{
					CRect rect = u->layer.GetPosition();
					u->layer.Swap(*image.GetLayer(u->i1));
					rect |= u->layer.GetPosition();
					Redraw(rect);
				}
				if (u->type == UndoLayerSelection) {
					CRect rect = selection.GetPosition();
					CleanSelection();
					selection.MoveTo(u->i2, u->i3);
					PrepareSelection();
					u->i2 = rect.left, u->i3 = rect.top;
				}
				break;

			case UndoImage:
				u->image.Swap(image);
				Redraw(true);
				break;

			case UndoNewLayer:
				{
					FGXLayer *layer = image.InsertLayer(u->i1);
					u->layer.Clone(*layer);
				}
				Redraw(u->layer.GetPosition());
				break;

			case UndoDeleteLayer:
				image.GetLayer(u->i1)->Swap(u->layer);
				image.DeleteLayer(u->i1);
				Redraw(u->layer.GetPosition());
				break;

			case UndoCanvasSize:
				image.ResizeCanvas(u->i1, u->i2, u->i3);
				u->i1 = bounds.right, u->i2 = bounds.bottom;
				Redraw(true);
				break;

			case UndoModifyLayer:
				{
					FGXLayer *layer = image.GetLayer(u->i1);

					BYTE o = layer->GetOpacity();
					layer->SetOpacity(u->i2);
					u->i2 = o;

					BYTE m = layer->GetMode();
					layer->SetMode(u->i3);
					u->i3 = m;

					swap(layer->a1, u->i4);
					swap(layer->a2, u->i5);
					swap(layer->a3, u->i6);
					swap(layer->a4, u->s1);
					swap(layer->a5, u->s2);

					if (layer->GetType() == LayerText) {
						layer->RenderText();
						Redraw();
					} else
						RedrawLayer(u->i1);
				}
				break;

			case UndoMoveLayer:
				{
					FGXLayer *layer = image.GetLayer(u->i1);
					CRect rect = layer->GetPosition();
					layer->MoveTo(u->i2, u->i3);
					u->i2 = rect.left, u->i3 = rect.top;
				}
				Redraw();
				break;

			case UndoMoveSelection:
				CleanSelection();
				{
					CRect rect = selection.GetPosition();
					selection.MoveTo(u->i1, u->i2);
					u->i1 = rect.left, u->i2 = rect.top;
				}
				PrepareSelection();
				break;

			case UndoAddLayerMask:
				image.GetLayer(u->i1)->AddMask();
				if (selection.GetPosition().IsRectEmpty() == false) {
					image.GetLayer(u->i1)->SetMask(selection, selection.GetPosition());
					RedrawLayer(u->i1);
				}
				break;

			case UndoRevealAll:
				image.RevealAll();
				Redraw(true);
				break;

			case UndoFlipImage:
				if (u->i1 == 0)
					image.Flip(true, false);
				else
					image.Flip(false, true);
				Redraw(true);
				break;

			case UndoOrderLayer:
				swap(u->i1, u->i2);
				image.MoveLayer(u->i1, u->i2);
				RedrawLayer(u->i2);
				break;

			case UndoRasterize:
				image.GetLayer(u->i1)->type = LayerNormal;
				RedrawLayer(u->i1);
				break;
		}

		return u->type;
	}

	return -1;
}

void CFotografixDoc::PurgeUndo() {
	for (int i = 0; i < undo.size(); i++)
		delete undo[i];

	undo.clear();
	undoPos = -1;
}

void CFotografixDoc::Redraw(bool sizeChanged) {
	if (sizeChanged) {
		bounds = CRect(0, 0, image.GetWidth(), image.GetHeight());
		bitmap.Allocate(bounds.Width(), bounds.Height());
	}

	image.Render(bitmap, bounds, channelMask);
	
	if (selection.GetPosition().IsRectEmpty() == false) {
		selection.InitAnimate(bitmap, bounds);
		selection.Animate(bitmap, bounds);
	}

	UpdateAllViews(NULL, sizeChanged);
}

void CFotografixDoc::Redraw(const CRect &rect) {
	image.Render(bitmap, rect, channelMask);

	if (selection.GetPosition().IsRectEmpty() == false) {
		selection.InitAnimate(bitmap, bounds);
		selection.Animate(bitmap, bounds);
	}

	UpdateAllViews(NULL);
}

void CFotografixDoc::CleanSelection() {
	if (selection.GetPosition().IsRectEmpty() == false) {
		image.Render(bitmap, selection.GetPosition(), channelMask);
		UpdateAllViews(NULL);
	}
}

void CFotografixDoc::PrepareSelection() {
	selection.InitAnimate(bitmap, bounds);
	selection.Animate(bitmap, bounds);
	UpdateAllViews(NULL);
}







// ------ 

#pragma pack(push, 1)

struct PsdHeader {
   BYTE Signature[4];   /* File ID "8BPS" */
   WORD Version;        /* Version number, always 1 */
   BYTE Reserved[6];    /* Reserved, must be zeroed */
   WORD Channels;       /* Number of color channels (1-24) including alpha
                           channels */
   LONG Rows;           /* Height of image in pixels (1-30000) */
   LONG Columns;        /* Width of image in pixels (1-30000) */
   WORD Depth;          /* Number of bits per channel (1, 8, and 16) */
   WORD Mode;           /* Color mode */
};

struct PsdResBlock {
	BYTE Signature[4];
	WORD Id;
};

struct PsdResolution {
	WORD hResW, hResF;
	WORD hResUnit;
	WORD WidthUnit;
	WORD vResW, vResF;
	WORD vResUnit;
	WORD HeightUnit;
};

struct PsdLayer {
	LONG Top;
	LONG Left;
	LONG Bottom;
	LONG Right;
	WORD Channels;
};

struct PsdLayer2 {
	BYTE Signature[4];
	LONG Blend;
	BYTE Opacity;
	BYTE Clipping;
	BYTE Flags;
	BYTE Padding;
	LONG ExtraDataSize;
};

struct PsdMask {
	LONG Top;
	LONG Left;
	LONG Bottom;
	LONG Right;
	BYTE Color;
	BYTE Flags;
	WORD Padding;
};

#pragma pack(pop)

void CFotografixDoc::PSD_ReadChannelData(CFile &file, FGXChannel &channel) {
	const CRect &rect = channel.GetPosition();
	FGXAccessor c(channel, rect);
	file.Read(&*c, rect.Width() * rect.Height());
}

void CFotografixDoc::PSD_ReadChannelDataRLE(CFile &file, FGXChannel &channel) {
	const CRect &rect = channel.GetPosition();
	FGXAccessor c(channel, rect);
	BYTE count, data;
	int done = 0, total = rect.Width() * rect.Height();

	while (done < total) {
		file.Read(&count, 1);
		if (count & 0x80) {
			count = -count + 1;

			file.Read(&data, 1);
			for (BYTE i = 0; i < count; i++, c++) *c = data;
		} else {
			count++;
			file.Read(&*c, count);
			c += count;
		}
		done += count;
	}
}

CString CFotografixDoc::PSD_ReadString(CFile &file) {
	BYTE Length;
	char String[256];
	wchar_t WString[256];

	file.Read(&Length, 1);
	file.Read(String, Length % 2 == 0 ? Length + 1 : Length);

	String[Length] = 0;
	mbstowcs(WString, String, Length + 1);

	return WString;
}

void CFotografixDoc::PSD_ReadChannel(CFile &file, FGXChannel &channel, short Compression) {
	if (Compression == -1) {
		// Read compression flag
		file.Read(&Compression, 2);

		if (Compression == 256) {
			// RLE data: Skip row lengths
			file.Seek(channel.GetPosition().Height() * 2, CFile::current);
		}
	}

	if (Compression == 256)
		PSD_ReadChannelDataRLE(file, channel);
	else
		PSD_ReadChannelData(file, channel);
}

// Rotates an array of pointers up, moving first element to the bottom
void CFotografixDoc::ChannelRotate(FGXChannel **base, int count) {
	FGXChannel *temp = base[0];

	for (int i = 0; i < count-1; i++)
		base[i] = base[i+1];

	base[count-1] = temp;
}

void CFotografixDoc::PSD_ReadLayer(CFile &file, FGXLayer &layer, PsdLayer &lrec, short Mode, bool SeparateFlags) {
	short Compression;

	CRect rect(lrec.Left, lrec.Top, lrec.Right, lrec.Bottom);
	layer.SetPosition(rect);

	if (SeparateFlags)
		Compression = -1;
	else {
		// Read single compression flag
		file.Read(&Compression, 2);

		if (Compression == 256) {
			// RLE data: Skip row lengths
			file.Seek(rect.Height() * lrec.Channels * 2, CFile::current);
		}
	}

	FGXChannel *channel[24] = { NULL };
	for (int i = 0; i < lrec.Channels; i++) {
		channel[i] = new FGXChannel;
		channel[i]->SetPosition(rect);
		channel[i]->Allocate();
		PSD_ReadChannel(file, *channel[i], Compression);
	}

	switch (Mode) {
		case 1:
			// Grayscale image
			if (lrec.Channels >= 1) {
				// Layer mask?
				if (lrec.Channels == 3) {
					layer.AddMask();
					channel[2]->Transfer(layer.channels[4]);
				}

				if (SeparateFlags && lrec.Channels >= 2) ChannelRotate(channel, 2);

				channel[0]->Transfer(layer.channels[1]);
				layer.channels[1].Clone(layer.channels[2]);
				layer.channels[1].Clone(layer.channels[3]);

				if (lrec.Channels >= 2)
					channel[1]->Transfer(layer.channels[0]);
				else
					layer.channels[0].Fill(255);
			}
			break;

		case 3:
			// RGB image
			if (lrec.Channels >= 3) {
				// Layer mask?
				if (lrec.Channels == 5) {
					layer.AddMask();
					channel[4]->Transfer(layer.channels[4]);
				}

				if (SeparateFlags && lrec.Channels >= 4) ChannelRotate(channel, 4);

				for (int i = 1; i <= 3; i++)
					channel[i-1]->Transfer(layer.channels[i]);

				if (lrec.Channels >= 4)
					channel[3]->Transfer(layer.channels[0]);
				else
					layer.channels[0].Fill(255);
			}
			break;

		case 4:
			// CMYK image
			if (lrec.Channels >= 4) {
				// Layer mask?
				if (lrec.Channels == 6) {
					layer.AddMask();
					channel[5]->Transfer(layer.channels[4]);
				}

				if (SeparateFlags && lrec.Channels >= 5) ChannelRotate(channel, 5);

				for (int i = 1; i <= 3; i++)
					channel[i-1]->Transfer(layer.channels[i]);

				// Read the K channel
				int n = rect.Width() * rect.Height();
				for (int i = 1; i <= 3; i++) {
					FGXAccessorC k(*channel[3], rect);
					FGXAccessor  c(layer.channels[i], rect);

					for (int j = 0; j < n; j++, k++, c++)
						*c = FGXBlend(*c, *k, 0);
				}

				if (lrec.Channels >= 5)
					channel[4]->Transfer(layer.channels[0]);
				else
					layer.channels[0].Fill(255);
			}
			break;
	}

	for (int i = 0; i < 24; i++) if (channel[i] != NULL) delete channel[i];
}

int CFotografixDoc::LoadImage_PSD(LPCTSTR path, FGXImage &image) {
	CFile file;
	if (file.Open(path, CFile::modeRead | CFile::shareDenyWrite) == false)
		return Status::AccessDenied;

	PsdHeader header;
	file.Read(&header, sizeof(header));

	// Check signature
	header.Mode = bswap(header.Mode);
	if (header.Version == 256 && header.Depth == 2048 && (header.Mode == 1 || header.Mode == 3 || header.Mode == 4 || header.Mode == 8) && memcmp(header.Signature, "8BPS", 4) == 0) {
		// Treat duotone image as grayscale
		if (header.Mode == 8) {
			header.Mode = 1;
			AfxGetMainWnd()->MessageBox(LangMessageParam(WarnPSDDuotone, path), NULL, MB_ICONWARNING | MB_OK);
		} else if (header.Mode == 4)
			AfxGetMainWnd()->MessageBox(LangMessageParam(WarnPSDCMYK, path), NULL, MB_ICONWARNING | MB_OK);

		// Convert byte order of header fields
		header.Channels = bswap(header.Channels);

		// Set image dimensions
		image.Initialize(bswap(header.Columns), bswap(header.Rows));

		int BlockLen;
		int AltStart;

		// Skip CMD block
		file.Read(&BlockLen, 4);
		file.Seek(bswap(BlockLen), CFile::current);

		// Read image resources block
		file.Read(&BlockLen, 4);
		AltStart = file.GetPosition() + bswap(BlockLen);

		while (file.GetPosition() < AltStart) {
			PsdResBlock block;
			file.Read(&block, sizeof(block));

			if (memcmp(block.Signature, "8BIM", 4) != 0)
				return Status::UnknownImageFormat;

			PSD_ReadString(file);

			file.Read(&BlockLen, 4);
			BlockLen = bswap(BlockLen);
			if (BlockLen % 2 == 1) BlockLen++;

			switch (bswap(block.Id)) {
			case 0x03ed:
				// Resolution info
				if (BlockLen == sizeof(PsdResolution)) {
					PsdResolution res;
					file.Read(&res, sizeof(res));
					image.SetResolution(bswap(res.hResW));
					break;
				}

			default:
				file.Seek(BlockLen, CFile::current);
				break;
			}
		}

		file.Seek(AltStart, CFile::begin);

		// Save file pointer for alternate stream
		file.Read(&BlockLen, 4);
		AltStart = file.GetPosition() + bswap(BlockLen);

		file.Read(&BlockLen, 4);

		// Read layer count
		short LayerCount;
		file.Read(&LayerCount, 2);
		LayerCount = bswap((unsigned short)LayerCount);
		if (LayerCount < 0) LayerCount = -LayerCount;

		if (LayerCount) {
			PsdLayer *lrec = new PsdLayer[LayerCount];
			PsdLayer2 *lrec2 = new PsdLayer2[LayerCount];
			CString *lname = new CString[LayerCount];

			for (int i = 0; i < LayerCount; i++) {
				// Read layer record
				file.Read(&lrec[i], sizeof(PsdLayer));
				lrec[i].Top = bswap(lrec[i].Top);
				lrec[i].Left = bswap(lrec[i].Left);
				lrec[i].Bottom = bswap(lrec[i].Bottom);
				lrec[i].Right = bswap(lrec[i].Right);
				lrec[i].Channels = bswap(lrec[i].Channels);

				// Skip channel length info records
				file.Seek(6 * lrec[i].Channels, CFile::current);

				// Read remainder of record
				file.Read(&lrec2[i], sizeof(PsdLayer2));

				// Save current file pointer
				int pos = file.GetPosition();

				// Skip mask info block
				file.Read(&BlockLen, 4);
				file.Seek(bswap(BlockLen), CFile::current);

				// Skip blending data
				file.Read(&BlockLen, 4);
				file.Seek(bswap(BlockLen), CFile::current);

				// Read layer name
				lname[i] = PSD_ReadString(file);

				// Jump to next layer record
				file.Seek(pos + bswap(lrec2[i].ExtraDataSize), CFile::begin);
			}

			for (int i = 0; i < LayerCount; i++) {
				FGXLayer &layer = *image.AddLayer();
				PSD_ReadLayer(file, layer, lrec[i], header.Mode, true);

				layer.SetName(lname[i]);
				layer.SetOpacity(lrec2[i].Opacity);
				layer.SetVisible((lrec2[i].Flags & 2) == 0);

				switch (lrec2[i].Blend) {
				case MAKESIG('m', 'u', 'l', ' '):
					layer.SetMode(ModeMultiply);
					break;

				case MAKESIG('s', 'c', 'r', 'n'):
					layer.SetMode(ModeScreen);
					break;

				case MAKESIG('o', 'v', 'e', 'r'):
					layer.SetMode(ModeOverlay);
					break;

				case MAKESIG('h', 'L', 'i', 't'):
					layer.SetMode(ModeHardLight);
					break;
				}
			}

			delete [] lname;
			delete [] lrec2;
			delete [] lrec;

			return Status::Ok;
		}

		// Jump to alternate stream
		file.Seek(AltStart, CFile::begin);

		// Add default layer
		FGXLayer *layer = new FGXLayer(CRect(0, 0, image.GetWidth(), image.GetHeight()));
		layer->SetName(LayerCount ? LangItem(FlattenedImage) : LangItem(Background));
		image.AddLayer(layer);

		PsdLayer lrec;
		lrec.Left = lrec.Top = 0;
		lrec.Right = image.GetWidth();
		lrec.Bottom = image.GetHeight();
		lrec.Channels = header.Channels;

		PSD_ReadLayer(file, *layer, lrec, header.Mode, false);

		return Status::Ok;
	}

	return Status::UnknownImageFormat;
}

#pragma pack(push, 1)

struct XCFHeader
{
	BYTE Magic[9];
	LONG Version;
	BYTE Null;
	LONG Width;
	LONG Height;
	LONG Mode;
};

struct XCFLayerHeader {
	LONG Width;
	LONG Height;
	LONG Mode;
};

#pragma pack(pop)

CString CFotografixDoc::XCF_ReadString(CFile &file) {
	int Length;
	char String[256];
	wchar_t WString[256];

	file.Read(&Length, sizeof(Length));
	if (Length == 0) return CString();

	Length = bswap(Length);
	file.Read(String, min(Length, 256));
	if (Length > 256) {
		String[255] = 0;
		file.Seek(Length - 256, CFile::current);
	}

	String[Length] = 0;
	mbstowcs(WString, String, Length + 1);

	return WString;
}

void CFotografixDoc::XCF_ReadHierarchy(CFile &file, BYTE **channelData, int Compression) {
	int Width, Height, Channels;

	file.Read(&Width, 4); Width = bswap(Width);
	file.Read(&Height, 4); Height = bswap(Height);
	file.Read(&Channels, 4); Channels = bswap(Channels);

	int pos;
	file.Read(&pos, 4);
	file.Seek(bswap(pos) + 8, CFile::begin);
	file.Read(&pos, 4);
	file.Seek(bswap(pos), CFile::begin);

	int nx = (Width + 63) / 64,
		ny = (Height + 63) / 64;

	// Process each tile
	for (int j = 0; j < ny; j++)
		for (int i = 0; i < nx; i++) {
			int w = (i < nx - 1) ? 64 : ((Width % 64) == 0 ? 64 : (Width % 64)),
				h = (j < ny - 1) ? 64 : ((Height % 64) == 0 ? 64 : (Height % 64)),
				n = w * h;

			BYTE *tile = new BYTE[n];

			for (int k = 0; k < Channels; k++) {
				// Decode tile data
				if (Compression == 0)
					file.Read(tile, n);
				else {
					int done = 0;
					BYTE *p = tile;
					BYTE count;
					unsigned short q;
					BYTE v;

					while (done < n) {
						file.Read(&count, 1);

						if (count <= 126) {
							count++;
							file.Read(&v, 1);
							for (int i = 0; i < count; i++) *p++ = v;
							done += count;
						} else if (count == 127) {
							file.Read(&q, 2);
							q = bswap(q);
							file.Read(&v, 1);
							for (int i = 0; i < q; i++) *p++ = v;
							done += q;
						} else if (count == 128) {
							file.Read(&q, 2);
							q = bswap(q);
							file.Read(p, q);
							p += q;
							done += q;
						} else {
							file.Read(p, 256 - count);
							p += 256 - count;
							done += 256 - count;
						}
					}

					v = 0;
				}

				// Copy tile to channel
				CRect rect(CPoint(i * 64, j * 64), CSize(w, h));
				BYTE *p, *q = tile;
				for (int j = rect.top; j < rect.bottom; j++) {
					p = channelData[k] + j * Width + rect.left;

					for (int i = rect.left; i < rect.right; i++, p++, q++)
						*p = *q;
				}
			}

			delete [] tile;
		}
}

bool CFotografixDoc::XCF_ReadLayer(CFile &file, FGXLayer &layer, int Mode, int Compression) {
	BYTE *channelData[4];

	switch (Mode) {
	case 0:
		layer.channels[0].Fill(255);
		channelData[0] = layer.channels[1].data;
		channelData[1] = layer.channels[2].data;
		channelData[2] = layer.channels[3].data;
		XCF_ReadHierarchy(file, channelData, Compression);
		return true;

	case 1:
		channelData[0] = layer.channels[1].data;
		channelData[1] = layer.channels[2].data;
		channelData[2] = layer.channels[3].data;
		channelData[3] = layer.channels[0].data;
		XCF_ReadHierarchy(file, channelData, Compression);
		return true;

	case 2:
		layer.channels[0].Fill(255);
		channelData[0] = layer.channels[1].data;
		XCF_ReadHierarchy(file, channelData, Compression);
		layer.channels[1].Clone(layer.channels[2]);
		layer.channels[1].Clone(layer.channels[3]);
		return true;

	case 3:
		channelData[0] = layer.channels[1].data;
		channelData[1] = layer.channels[0].data;
		XCF_ReadHierarchy(file, channelData, Compression);
		layer.channels[1].Clone(layer.channels[2]);
		layer.channels[1].Clone(layer.channels[3]);
		return true;

	default:
		return false;
	}
}

int CFotografixDoc::LoadImage_XCF(LPCTSTR path, FGXImage &image) {
	CFile file;
	if (file.Open(path, CFile::modeRead | CFile::shareDenyWrite) == false)
		return Status::AccessDenied;

	// Read header
	XCFHeader header;
	file.Read(&header, sizeof(header));

	header.Mode = bswap(header.Mode);

	if (memcmp(header.Magic, "gimp xcf ", 9) != 0 || header.Null != 0 || header.Mode > 1)
		return Status::UnknownImageFormat;

	// Initialize image dimensions
	image.Initialize(bswap(header.Width), bswap(header.Height));

	FGXTriple palette[256];
	BYTE Compression;

	// Read property list
	{
		int type;
		int size;

		file.Read(&type, sizeof(type)); type = bswap(type);
		file.Read(&size, sizeof(size)); size = bswap(size);

		while (type != 0) {
			switch (type) {
			case 1:	// Color map
				{
					int n;
					file.Read(&n, sizeof(n));
					n = bswap(n);

					for (int i = 0; i < n; i++)
						file.Read(&palette[i], 3);
				}
				break;

			case 17: // Compression
				file.Read(&Compression, 1);
				break;

			case 19: // Resolution
				{
					float x, y;
					file.Read(&x, sizeof(x));
					file.Read(&y, sizeof(y));

					int res = bswap(*(int *)&x);
					image.SetResolution(*(float *)&res);
				}
				break;

			default:
				file.Seek(size, CFile::current);
				break;
			}

			if (file.Read(&type, sizeof(type)) == 0 || file.Read(&size, sizeof(size)) == 0)
				break;

			type = bswap(type);
			size = bswap(size);
		}
	}

	// Read layer records
	while (1) {
		int ptr;

		if (file.Read(&ptr, sizeof(ptr)) == 0)
			return Status::WrongState;
		else if (ptr == 0)
			break;

		int pos = file.GetPosition();
		file.Seek(bswap(ptr), CFile::begin);

		XCFLayerHeader lheader;
		file.Read(&lheader, sizeof(lheader));

		FGXLayer &layer = *image.InsertLayer(0);
		layer.SetName(XCF_ReadString(file));

		CRect rect(0, 0, bswap(lheader.Width), bswap(lheader.Height));
		bool applyMask = false;

		// Read property list
		{
			int type;
			int size;

			file.Read(&type, sizeof(type)); type = bswap(type);
			file.Read(&size, sizeof(size)); size = bswap(size);

			while (type != 0) {
				switch (type) {
				case 6: // Opacity
					{
						int opacity;
						file.Read(&opacity, sizeof(opacity));
						layer.SetOpacity(bswap(opacity));
					}
					break;

				case 7:	// Blending mode
					{
						int mode;
						file.Read(&mode, sizeof(mode));

						switch (bswap(mode)) {
						case 3:
							layer.SetMode(ModeMultiply);
							break;

						case 4:
							layer.SetMode(ModeScreen);
							break;

						case 5:
							layer.SetMode(ModeOverlay);
							break;

						case 18:
							layer.SetMode(ModeHardLight);
							break;
						}
					}
					break;

				case 8: // Visible
					{
						int visible;
						file.Read(&visible, sizeof(visible));
						layer.SetVisible(bswap(visible) == 1);
					}
					break;

				case 11: // Apply mask
					{
						int apply;
						file.Read(&apply, sizeof(apply));
						applyMask = bswap(apply) == 1;
					}
					break;

				case 15: // Offsets
					{
						int dx, dy;
						file.Read(&dx, sizeof(dx));
						file.Read(&dy, sizeof(dy));

						rect.MoveToXY(bswap(dx), bswap(dy));
					}
					break;

				default:
					file.Seek(size, CFile::current);
					break;
				}

				if (file.Read(&type, sizeof(type)) == 0 || file.Read(&size, sizeof(size)) == 0)
					break;

				type = bswap(type);
				size = bswap(size);
			}

			layer.SetPosition(rect);

			int hptr, mptr;
			file.Read(&hptr, sizeof(hptr));
			file.Read(&mptr, sizeof(mptr));

			// Read channel data
			file.Seek(bswap(hptr), CFile::begin);
			if (XCF_ReadLayer(file, layer, bswap(lheader.Mode), Compression) == false)
				return Status::UnknownImageFormat;

			// Read mask, if present
			if (mptr != 0) {
				file.Seek(bswap(mptr) + 8, CFile::begin);
				XCF_ReadString(file);

				// Skip property list
				{
					int temp;
					while (file.Read(&temp, sizeof(temp)) != 0 && temp != 0)
						file.Read(&temp, sizeof(temp));
				}

				file.Read(&ptr, sizeof(ptr));
				file.Seek(bswap(ptr), CFile::begin);

				layer.AddMask();
				layer.EnableMask(applyMask);

				BYTE *channelData[1] = { layer.channels[4].data };
				XCF_ReadHierarchy(file, channelData, Compression);
			}
		}

		file.Seek(pos, CFile::begin);
	}

	return Status::Ok;
}

#pragma pack(push, 1)

struct PcxHeader
{
  BYTE	Identifier;        /* PCX Id Number (Always 0x0A) */
  BYTE	Version;           /* Version Number */
  BYTE	Encoding;          /* Encoding Format */
  BYTE	BitsPerPixel;      /* Bits per Pixel */
  WORD	XStart;            /* Left of image */
  WORD	YStart;            /* Top of Image */
  WORD	XEnd;              /* Right of Image */
  WORD	YEnd;              /* Bottom of image */
  WORD	HorzRes;           /* Horizontal Resolution */
  WORD	VertRes;           /* Vertical Resolution */
  BYTE	Palette[48];       /* 16-Color EGA Palette */
  BYTE	Reserved1;         /* Reserved (Always 0) */
  BYTE	NumBitPlanes;      /* Number of Bit Planes */
  WORD	BytesPerLine;      /* Bytes per Scan-line */
  WORD	PaletteType;       /* Palette Type */
  WORD	HorzScreenSize;    /* Horizontal Screen Size */
  WORD	VertScreenSize;    /* Vertical Screen Size */
  BYTE	Reserved2[54];     /* Reserved (Always 0) */
};

#pragma pack(pop)

int CFotografixDoc::LoadImage_PCX(LPCTSTR path, FGXImage &image) {
	CFile file;
	if (file.Open(path, CFile::modeRead | CFile::shareDenyWrite) == false)
		return Status::AccessDenied;

	// Currently supports only 256-colour images
	PcxHeader header;
	file.Read(&header, sizeof(header));

	// Check header validity
	if (header.Identifier == 0x0a && header.Version == 5 && header.Encoding == 1 && header.BitsPerPixel == 8 && header.NumBitPlanes == 1) {
		// Get image dimensions
		if (header.HorzRes > 0) image.SetResolution(header.HorzRes);
		image.Initialize(header.XEnd + 1, header.YEnd + 1);

		// Ensure that dimensions are valid
		if (image.GetWidth() > 0 && image.GetHeight() > 0) {
			// Calculate scan length
			int scanlen = header.BytesPerLine;
			int stride = (scanlen << 1) - image.GetWidth();
			int np = image.GetWidth() * image.GetHeight();

			// Decode RLE data
			BYTE *buffer = new BYTE[np], *p, *end = buffer + np;

			char data;
			int i, j, l = 0, count;
			for (p = buffer; p < end;) {
				i = 0;
				l++;

				while (i < scanlen) {
					file.Read(&data, 1);
					if ((data & 0xc0) == 0xc0) {
						count = data & 0x3f;
						file.Read(&data, 1);
						for (j = 0; j < count; j++) *(p++) = data;
						i += count;
					} else {
						*(p++) = data;
						i++;
					}
				}
			}

			// Check if VGA palette is present
			FGXPalette pal;
			if (header.Version == 5) {
				char id = 0;

				file.Seek(-769, CFile::end);
				file.Read(&id, 1);
				if (id == 0x0c) {
					// Load VGA palette
					file.Read(&pal, 768);
				} else {
					delete [] buffer;
					return Status::UnknownImageFormat;
				}
			}

			FGXLayer *layer = image.AddLayer();
			layer->SetPosition(CRect(header.XStart, header.YStart, header.XEnd + 1, header.YEnd + 1));

			// Decode pixel data
			FGXTriple *rgb = new FGXTriple[np], *tri = rgb;
			for (p = buffer; p < end; p++, tri++) *tri = pal[*p];

			delete [] buffer;
			layer->LoadFromMemory(rgb, 3);
			delete [] rgb;

			return Status::Ok;
		}
	}

	return Status::UnknownImageFormat;
}

#pragma pack(push, 1)
struct TgaHeader
{
  BYTE IDLength;        /* 00h  Size of Image ID field */
  BYTE ColorMapType;    /* 01h  Color map type */
  BYTE ImageType;       /* 02h  Image type code */
  WORD CMapStart;       /* 03h  Color map origin */
  WORD CMapLength;      /* 05h  Color map length */
  BYTE CMapDepth;       /* 07h  Depth of color map entries */
  WORD XOffset;         /* 08h  X origin of image */
  WORD YOffset;         /* 0Ah  Y origin of image */
  WORD Width;           /* 0Ch  Width of image */
  WORD Height;          /* 0Eh  Height of image */
  BYTE PixelDepth;      /* 10h  Image pixel size */
  BYTE ImageDescriptor; /* 11h  Image descriptor byte */
};
#pragma pack(pop)

int CFotografixDoc::LoadImage_TGA(LPCTSTR path, FGXImage &image) {
	CFile file;
	if (file.Open(path, CFile::modeRead | CFile::shareDenyWrite) == false)
		return Status::AccessDenied;

	// Currently supports only 24-bit and 32-bit raw TGA images
	TgaHeader header;
	file.Read(&header, sizeof(header));

	// Check header validity
	if (header.ColorMapType == 0 && (header.ImageType == 2 || header.ImageType == 10) && (header.PixelDepth == 24 || header.PixelDepth == 32) && header.Width > 0 && header.Height > 0) {
		// Get image dimensions
		image.Initialize(header.XOffset + header.Width, header.YOffset + header.Height);

		// Skip ID
		file.Seek(header.IDLength, CFile::current);

		FGXLayer *layer = image.AddLayer();
		layer->SetPosition(CRect(header.XOffset, header.YOffset, image.GetWidth(), image.GetHeight()));

		int n = header.Width * header.Height;

		// Read pixel data
		if (header.PixelDepth == 32) {
			// 32-bit data
			DWORD *bits = new DWORD[n];

			if (header.ImageType == 2)
				// Unencoded data
				file.Read(bits, n * 4);
			else {
				// Run-length encoded data
				DWORD *p = bits, *end = p + n, save;
				BYTE count;
				while (p < end) {
					file.Read(&count, 1);
					if (count & 0x80) {
						count = (count & 0x7f) + 1;
						file.Read(&save, 4);
						for (int i = 0; i < count; i++, p++) *p = save;
					} else {
						count++;
						file.Read(p, count * 4);
						p += count;
					}
				}
			}

			layer->LoadFromMemory(bits, 4);
			delete bits;
		} else {
			// 24-bit data
			FGXTriple *buffer = new FGXTriple[n], *tri = buffer;

			if (header.ImageType == 2) {
				// Unencoded data
				file.Read(buffer, n * 3);
				for (int i = 0; i < n; i++, tri++) swap(tri->r, tri->b);
			} else {
				// Run-length encoded data
				FGXTriple *bits = buffer, *end = bits + n, save;
				BYTE count;
				while (bits < end) {
					file.Read(&count, 1);
					if (count & 0x80) {
						count = (count & 0x7f) + 1;
						file.Read(&save, 3);
						swap(save.r, save.b);
						for (int i = 0; i < count; i++, bits++) *bits = save;
					} else {
						count++;
						file.Read(bits, count * 3);
						for (int i = 0; i < count; i++, bits++) swap(bits->r, bits->b);
					}
				}
			}

			layer->LoadFromMemory(buffer, 3);
			delete [] buffer;
		}

		image.Flip((header.ImageDescriptor & 16) != 0, (header.ImageDescriptor & 32) == 0);
		return Status::Ok;
	}

	return Status::UnknownImageFormat;
}

int CFotografixDoc::LoadImage_ICO(LPCTSTR path, FGXImage &image) {
	HICON hIcon = (HICON)::LoadImage(NULL, path, IMAGE_ICON, 0, 0, LR_LOADFROMFILE);

	if (hIcon) {
		bool result = LoadImage_Icon(hIcon, image);
		::DestroyIcon(hIcon);
		return result;
	}

	return Status::UnknownImageFormat;
}

int CFotografixDoc::LoadImage_EXE(LPCTSTR path, FGXImage &image) {
	SHFILEINFO shfi;

	if (::SHGetFileInfo(path, 0, &shfi, sizeof(shfi), SHGFI_ICON | SHGFI_LARGEICON)) {
		bool result = LoadImage_Icon(shfi.hIcon, image);
		::DestroyIcon(shfi.hIcon);
		return result;
	}

	return Status::AccessDenied;
}

int CFotografixDoc::LoadImage_RAW(LPCTSTR path, FGXImage &image) {
	CFile file;
	if (file.Open(path, CFile::modeRead | CFile::shareDenyWrite) == false)
		return Status::AccessDenied;

	CRawDialog dlg;
	dlg.length = file.GetLength();

	if (dlg.DoModal() == IDOK) {
		file.Seek(dlg.header, CFile::begin);

		int n = dlg.w * dlg.h * 3;
		BYTE *p = new BYTE[n];
		file.Read(p, n);

		FGXLayer *layer = new FGXLayer(CRect(0, 0, dlg.w, dlg.h));
		layer->LoadFromMemory(p, 3);

		image.Initialize(dlg.w, dlg.h);
		image.AddLayer(layer);

		delete p;

		return Status::Ok;
	} else
		return Status::Aborted;
}

int CFotografixDoc::SaveImage_TGA(LPCTSTR path, const FGXImage &image) {
	static const char id[] = "Saved by Fotografix";

	CFile file;
	if (file.Open(path, CFile::modeCreate | CFile::modeWrite) == false)
		return Status::AccessDenied;

	// Prepare header
	TgaHeader header;
	ZeroMemory(&header, sizeof(header));
	header.IDLength = sizeof(id);
	header.ImageType = 2;
	header.Width = image.GetWidth();
	header.Height = image.GetHeight();
	header.PixelDepth = 32;
	header.ImageDescriptor = 0x28;

	file.Write(&header, sizeof(header));
	file.Write(id, sizeof(id));

	// Write pixel data

	FGXLayer layer(CRect(0, 0, header.Width, header.Height));
	image.Render(layer, CRect(0, 0, header.Width, header.Height), true);

	int n = header.Width * header.Height;
	DWORD *p = new DWORD[n];
	layer.SaveToMemory(p, 4);
	file.Write(p, n*4);
	delete p;

	file.Close();

	return Status::Ok;
}

int CFotografixDoc::SaveImage_RAW(LPCTSTR path, const FGXImage &image) {
	CFile file;
	if (file.Open(path, CFile::modeCreate | CFile::modeWrite) == false)
		return Status::AccessDenied;

	// Write pixel data

	CRect rect = CRect(0, 0, image.GetWidth(), image.GetHeight());
	FGXLayer layer(rect);
	layer.Fill(FGXColor(globals.bgColor.GetColor(), 255));
	image.Render(layer, rect, false);

	int n = rect.Width() * rect.Height() * 3;
	BYTE *p = new BYTE[n];
	layer.SaveToMemory(p, 3);
	file.Write(p, n);
	delete p;

	file.Close();

	return Status::Ok;
}

#pragma pack(push, 1)

struct FGXBlockHeader {
	LONG Signature;			// Block type signature
	LONG BlockSize;			// Size of the data block that follows
};

struct FGXHeader {
	LONG Width;				// Width of image in pixels
	LONG Height;			// Height of image in pixels
	WORD Resolution;		// Resolution of image in pixels per inch
	WORD LayerCount;		// Number of layers in the image
	BYTE Compression;		// Compression method used for channel data (currently 0 = none)
	BYTE Unit;				// Dimension unit
};

struct FGXLayerHeader {
	RECT Position;			// Position of layer in the image
	BYTE Type;				// Layer type
	BYTE Channels;			// Number of channels in the layer
	BYTE Opacity;			// Opacity of the layer
	BYTE Mode;				// Blending mode of the layer
	BYTE Visible;			// Visibility of layer
	BYTE MaskEnabled;		// Indicates whether the layer mask is enabled
	BYTE Reserved;			// Reserved (must be 0)
	BYTE NameLength;		// Length of layer name that follows
};

struct FGXAdjustLayerHeader {
	LONG a;
	LONG a1;
	LONG a2;
	LONG a3;
};

struct FGXTextLayerHeader {
	LONG a1;
	LONG a2;
	LONG a3;
	WORD a4;
	WORD a5;
};

#pragma pack(pop)

int CFotografixDoc::LoadImage_FGX(LPCTSTR path, FGXImage &image) {
	CFile file;
	if (file.Open(path, CFile::modeRead | CFile::shareDenyWrite) == false)
		return Status::AccessDenied;

	FGXBlockHeader bh;
	bool extra = false;

	// Read first block header; must be image header (FGX2)
	if (file.Read(&bh, sizeof(bh)) != sizeof(bh) || bh.Signature != MAKESIG('F','G','X','2') || bh.BlockSize != sizeof(FGXHeader))
		return Status::UnknownImageFormat;

	FGXHeader header;
	file.Read(&header, sizeof(header));

	if (header.Compression != 0 || header.LayerCount < 1 || header.Width < 1 || header.Height < 1 || header.Resolution < 1)
		return Status::UnknownImageFormat;

	image.Initialize(header.Width, header.Height);
	image.SetResolution(header.Resolution);
	image.SetUnit(header.Unit);

	FGXLayer *layer = NULL;
	int channel;
	int channelSize;

	// Read data blocks
	while (file.Read(&bh, sizeof(bh)) == sizeof(bh))
		switch (bh.Signature) {
		case MAKESIG('F','G','X','L'):
			if (bh.BlockSize < sizeof(FGXLayerHeader))
				return Status::WrongState;
			else {
				// Read layer header
				FGXLayerHeader lheader;
				file.Read(&lheader, sizeof(lheader));

				if (bh.BlockSize != sizeof(lheader) + lheader.NameLength * sizeof(TCHAR) || lheader.Channels > 5)
					return Status::WrongState;

				layer = new FGXLayer(CRect(lheader.Position.left, lheader.Position.top, lheader.Position.right, lheader.Position.bottom), lheader.Type);

				if (lheader.Channels == 5) layer->AddMask();
				if (layer->numChannels != lheader.Channels) {
					delete layer;
					return Status::WrongState;
				}

				layer->SetOpacity(lheader.Opacity);
				layer->SetMode(lheader.Mode);
				layer->SetVisible(lheader.Visible);
				layer->EnableMask(lheader.MaskEnabled);

				// Read layer name
				file.Read(layer->name, lheader.NameLength * sizeof(TCHAR));

				image.AddLayer(layer);
				channel = 0;
				channelSize = layer->position.Width() * layer->position.Height();
			}
			break;

		case MAKESIG('F','G','X','C'):
			if (layer == NULL || channel >= layer->numChannels || bh.BlockSize != channelSize)
				return Status::WrongState;
			else
				file.Read(layer->channels[channel++].data, channelSize);
			break;

		case MAKESIG('F','G','X','A'):
			if (layer == NULL || layer->GetType() != LayerAdjust || bh.BlockSize != sizeof(FGXAdjustLayerHeader))
				return Status::WrongState;
			else {
				FGXAdjustLayerHeader alh;
				file.Read(&alh, sizeof(alh));

				layer->a = alh.a;
				layer->a1 = alh.a1;
				layer->a2 = alh.a2;
				layer->a3 = alh.a3;
			}
			break;

		case MAKESIG('F','G','X','T'):
			if (layer == NULL || layer->GetType() != LayerText || bh.BlockSize < sizeof(FGXTextLayerHeader))
				return Status::WrongState;
			else {
				FGXTextLayerHeader tlh;
				file.Read(&tlh, sizeof(tlh));

				if (bh.BlockSize != sizeof(FGXTextLayerHeader) + (tlh.a4 + tlh.a5) * sizeof(TCHAR))
					return Status::WrongState;

				layer->a1 = tlh.a1;
				layer->a2 = tlh.a2;
				layer->a3 = tlh.a3;

				file.Read(layer->a4.GetBuffer(tlh.a4), tlh.a4 * sizeof(TCHAR)), layer->a4.ReleaseBuffer();
				file.Read(layer->a5.GetBuffer(tlh.a5), tlh.a5 * sizeof(TCHAR)), layer->a5.ReleaseBuffer();
			}
			break;

		default:
			file.Seek(bh.BlockSize, CFile::current);
			extra = true;
			break;
		}

	if (image.GetLayerCount() < header.LayerCount)
		return Status::WrongState;

	return extra ? Status::NotImplemented : Status::Ok;
}

void CFotografixDoc::FGX_WriteBlockHeader(CFile &file, LONG Signature, LONG BlockSize) {
	FGXBlockHeader header = { Signature, BlockSize };
	file.Write(&header, sizeof(header));
}

int CFotografixDoc::SaveImage_FGX(LPCTSTR path, const FGXImage &image) {
	CFile file;
	if (file.Open(path, CFile::modeCreate | CFile::modeWrite) == false)
		return Status::AccessDenied;

	// Write image header
	FGXHeader header;
	header.Width = image.GetWidth();
	header.Height = image.GetHeight();
	header.Resolution = image.GetResolution();
	header.Unit = image.GetUnit();
	header.LayerCount = image.GetLayerCount();
	header.Compression = 0;
	FGX_WriteBlockHeader(file, MAKESIG('F','G','X','2'), sizeof(FGXHeader));
	file.Write(&header, sizeof(header));

	for (int i = 0; i < header.LayerCount; i++) {
		const FGXLayer &layer = *image.GetLayer(i);

		// Write layer header
		FGXLayerHeader lheader;

		const CRect &rect = layer.GetPosition();
		::SetRect(&lheader.Position, rect.left, rect.top, rect.right, rect.bottom);

		lheader.Type = layer.GetType();
		lheader.Channels = layer.numChannels;
		lheader.Opacity = layer.GetOpacity();
		lheader.Mode = layer.GetMode();
		lheader.Visible = layer.IsVisible();
		lheader.MaskEnabled = layer.IsMaskEnabled();
		lheader.Reserved = 0;

		LPCTSTR name = layer.GetName();
		lheader.NameLength = _tcslen(name) + 1;

		FGX_WriteBlockHeader(file, MAKESIG('F','G','X','L'), sizeof(FGXLayerHeader) + lheader.NameLength * sizeof(TCHAR));
		file.Write(&lheader, sizeof(lheader));
		file.Write(name, lheader.NameLength * sizeof(TCHAR));

		// Write additional layer data
		switch (lheader.Type) {
		case LayerAdjust:
			{
				FGXAdjustLayerHeader alh = { layer.a, layer.a1, layer.a2, layer.a3 };
				FGX_WriteBlockHeader(file, MAKESIG('F','G','X','A'), sizeof(FGXAdjustLayerHeader));
				file.Write(&alh, sizeof(alh));
			}
			break;

		case LayerText:
			{
				FGXTextLayerHeader tlh = { layer.a1, layer.a2, layer.a3, layer.a4.GetLength() + 1, layer.a5.GetLength() + 1 };
				FGX_WriteBlockHeader(file, MAKESIG('F','G','X','T'), sizeof(FGXTextLayerHeader) + (tlh.a4 + tlh.a5) * sizeof(TCHAR));
				file.Write(&tlh, sizeof(tlh));
				file.Write((LPCTSTR)layer.a4, tlh.a4 * sizeof(TCHAR));
				file.Write((LPCTSTR)layer.a5, tlh.a5 * sizeof(TCHAR));
			}
			break;
		}

		// Write channel data
		int n = rect.Width() * rect.Height();
		for (int j = 0; j < lheader.Channels; j++) {
			FGX_WriteBlockHeader(file, MAKESIG('F','G','X','C'), n);
			file.Write(layer.channels[j].data, n);
		}
	}

	file.Close();
	return Status::Ok;
}

BOOL CFotografixDoc::SaveModified()
{
	if (IsModified()) {
		switch (AfxGetMainWnd()->MessageBox(LangMessageParam(AskFileSave, GetTitle()), NULL, MB_ICONWARNING | MB_YESNOCANCEL)) {
		case IDYES:
			{
				POSITION pos = GetFirstViewPosition();
				static_cast<CFotografixView *>(GetNextView(pos))->OnFileSave();
			}
			if (IsModified() == true) return false;
			break;

		case IDNO:
			return true;

		case IDCANCEL:
			return false;
		}
	}

	if (dirty == true && (image.GetLayerCount() > 1 || image.GetLayer(0)->GetOpacity() < 255 || image.GetLayer(0)->HasMask()) &&
		GetPathName().Right(4).MakeLower() != TEXT(".fgx") &&
		AfxGetMainWnd()->MessageBox(LangMessage(WarnNoLayers), NULL, MB_ICONWARNING | MB_OKCANCEL) == IDCANCEL)
		return false;

	return true;
}
