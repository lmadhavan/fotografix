// FotografixView.h : interface of the CFotografixView class
//


#pragma once

#include "FGXBitmap.h"
#include "Fotografix.h"
#include "FotografixDoc.h"
#include "AdjustDialog.h"

class CFotografixView : public CScrollView
{
protected: // create from serialization only
	CFotografixView();
	DECLARE_DYNCREATE(CFotografixView)

// Attributes
public:
	CFotografixDoc* GetDocument() const;

// Operations
public:

// Overrides
public:
	virtual void OnDraw(CDC* pDC);  // overridden to draw this view

	afx_msg void OnSize(UINT nType, int cx, int cy);

	void DrawTool(CDC *pDC);
	afx_msg void OnLButtonDown(UINT nFlags, CPoint point);
	afx_msg void OnMouseMove(UINT nFlags, CPoint point);
	afx_msg void OnLButtonUp(UINT nFlags, CPoint point);
	afx_msg BOOL OnSetCursor(CWnd* pWnd, UINT nHitTest, UINT message);
	afx_msg BOOL OnMouseWheel(UINT nFlags, short zDelta, CPoint point);
	afx_msg void OnMButtonDown(UINT nFlags, CPoint point);
	afx_msg void OnMButtonUp(UINT nFlags, CPoint point);
	afx_msg void OnRButtonDown(UINT nFlags, CPoint point);
	afx_msg void OnRButtonUp(UINT nFlags, CPoint point);
	afx_msg void OnHScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar);
	afx_msg void OnVScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar);

	afx_msg void OnTimer(UINT_PTR nIDEvent);
	afx_msg LRESULT OnApp(WPARAM wParam, LPARAM lParam);

	// File menu

	afx_msg void OnFileSave();
	afx_msg void OnFileSaveAs();

	// Edit menu

	afx_msg void OnEditUndo();
	afx_msg void OnUpdateEditUndo(CCmdUI *pCmdUI);
	afx_msg void OnEditRedo();
	afx_msg void OnUpdateEditRedo(CCmdUI *pCmdUI);
	afx_msg void OnEditPurgeundo();
	afx_msg void OnUpdateEditPurgeundo(CCmdUI *pCmdUI);
	afx_msg void OnEditCut();
	afx_msg void OnEditCopy();
	afx_msg void OnUpdateEditCopy(CCmdUI *pCmdUI);
	afx_msg void OnEditPaste();
	afx_msg void OnUpdateEditPaste(CCmdUI *pCmdUI);
	afx_msg void OnEditPasteexternal();
	afx_msg void OnUpdateEditPasteexternal(CCmdUI *pCmdUI);
	afx_msg void OnEditClear();
	afx_msg void OnEditPurgeclipboard();
	afx_msg void OnEditPurgeall();
	afx_msg void OnUpdateEditPurgeall(CCmdUI *pCmdUI);
	afx_msg void OnEditFill();
	afx_msg void OnEditFillFg();
	afx_msg void OnEditFillBg();

	// Image menu

	afx_msg void OnImageCrop();
	afx_msg void OnImageRevealall();
	afx_msg void OnImageCanvassize();
	afx_msg void OnImageImagesize();
	afx_msg void OnFlipHorizontal();
	afx_msg void OnFlipVertical();
	afx_msg void OnRotate90();
	afx_msg void OnRotate180();
	afx_msg void OnRotate270();
	afx_msg void OnRotateOther();

	// Adjustments menu

	afx_msg void OnAdjustment(UINT nID);
	afx_msg void OnUpdateAdjustments(CCmdUI *pCmdUI);

	// Layer menu

	afx_msg void OnLayerNew();
	afx_msg void OnNewLayerviacopy();
	afx_msg void OnUpdateNewLayerviacopy(CCmdUI *pCmdUI);
	afx_msg void OnNewLayerviacut();
	afx_msg void OnLayerDuplicatelayer();
	afx_msg void OnLayerProperties();
	afx_msg void OnNewAdjustmentLayer(UINT nID);
	afx_msg void OnLayerDelete();
	afx_msg void OnLayerRename();
	afx_msg void OnUpdateLayerDelete(CCmdUI *pCmdUI);
	afx_msg void OnLayerAddlayermask();
	afx_msg void OnUpdateLayerAddlayermask(CCmdUI *pCmdUI);
	void OnLayerRemovelayermask_Helper(bool apply);
	afx_msg void OnLayerRemovelayermask();
	afx_msg void OnUpdateLayerRemovelayermask(CCmdUI *pCmdUI);
	afx_msg void OnLayerEnablelayermask();
	afx_msg void OnUpdateLayerEnablelayermask(CCmdUI *pCmdUI);
	afx_msg void OnLayerMoveup();
	afx_msg void OnLayerMovedown();
	afx_msg void OnLayerRasterize();
	afx_msg void OnUpdateLayerRasterize(CCmdUI *pCmdUI);
	afx_msg void OnLayerMergedown();
	afx_msg void OnUpdateLayerMergedown(CCmdUI *pCmdUI);
	afx_msg void OnLayerFlattenimage();
	afx_msg void OnUpdateLayerFlattenimage(CCmdUI *pCmdUI);

	// Select menu

	afx_msg void OnSelectAll();
	afx_msg void OnSelectDeselect();
	afx_msg void OnSelectInverse();
	afx_msg void OnUpdateSelectDeselect(CCmdUI *pCmdUI);
	afx_msg void OnSelectLayertransparency();
	void OnModify_Helper(int distance, LPCTSTR undoText);
	afx_msg void OnModifyExpand();
	afx_msg void OnModifyContract();
	void OnFeather_Helper(int radius);
	afx_msg void OnModifyFeather();

	// View menu

	afx_msg void OnViewZoomin();
	afx_msg void OnViewZoomout();
	afx_msg void OnViewScaletofit();
	afx_msg void OnViewActualsize();

	// Window menu

	afx_msg void OnUpdateWindowNew(CCmdUI *pCmdUI);

protected:
	virtual BOOL PreCreateWindow(CREATESTRUCT& cs);
	virtual void OnInitialUpdate(); // called first time after construct
	virtual void OnUpdate(CView* /*pSender*/, LPARAM /*lHint*/, CObject* /*pHint*/);
	virtual BOOL OnPreparePrinting(CPrintInfo* pInfo);
	virtual void OnBeginPrinting(CDC* pDC, CPrintInfo* pInfo);
	virtual void OnEndPrinting(CDC* pDC, CPrintInfo* pInfo);

// Implementation
public:
	virtual ~CFotografixView();
#ifdef _DEBUG
	virtual void AssertValid() const;
	virtual void Dump(CDumpContext& dc) const;
#endif

protected:

// Generated message map functions
protected:
	DECLARE_MESSAGE_MAP()

private:
	void ComputeSizes();
	void SetZoomFactor(float factor);

	void TransformPoint(CPoint &pt) {
		pt += GetScrollPosition();
		pt.x = (pt.x - dx) / zoom;
		pt.y = (pt.y - dy) / zoom;
	}

	void TransformRect(CRect &rect) {
		TransformPoint(rect.TopLeft());
		TransformPoint(rect.BottomRight());
	}

	void UntransformPoint(CPoint &pt) {
		pt.x = pt.x * zoom + dx;
		pt.y = pt.y * zoom + dy;
		pt -= GetScrollPosition();
	}

	void UntransformRect(CRect &rect) {
		UntransformPoint(rect.TopLeft());
		UntransformPoint(rect.BottomRight());
	}

	void Paint(FGXLayer &brush, FGXLayer &layer, const CPoint &pt) {
		brush.MoveTo(pt.x - brush.GetPosition().Width() / 2, pt.y - brush.GetPosition().Height() / 2);
		layer.EnsureRect(brush.GetPosition());
		brush.AlphaRender(layer, brush.GetPosition(), pDoc->channelMask);
		pDoc->Redraw(brush.GetPosition());
	}

	void Erase(FGXLayer &brush, FGXLayer &layer, const CPoint &pt) {
		brush.MoveTo(pt.x - brush.GetPosition().Width() / 2, pt.y - brush.GetPosition().Height() / 2);
		brush.Erase(layer, brush.GetPosition());
		pDoc->Redraw(brush.GetPosition());
	}

	void Clone(FGXLayer &brush, FGXLayer &layer, const CPoint &pt, const CPoint &source) {
		CPoint sourcePt(source.x - brush.GetPosition().Width() / 2, source.y - brush.GetPosition().Height() / 2);
		CRect rect(sourcePt, CSize(brush.GetPosition().Width(), brush.GetPosition().Height()));

		CPoint destPt(pt.x - brush.GetPosition().Width() / 2, pt.y - brush.GetPosition().Height() / 2);
		rect &= layer.GetPosition();

		brush.MoveTo(sourcePt.x, sourcePt.y);
		layer.CopyRect(brush, rect, ChannelAll);
		brush.MoveTo(destPt.x, destPt.y);

		rect.OffsetRect(destPt - sourcePt);
		layer.EnsureRect(rect);
		brush.AlphaRender(layer, rect, pDoc->channelMask);
		pDoc->Redraw(rect);
	}

	void SelectLayer(int i) {
		selLayer = i;
		layer = pDoc->image.GetLayer(i);
		if (pDoc->tActive == true) CalcHandles();
	}

	void CalcHandles();
	bool InitAdjustDialog(CAdjustDialog &dlg, int type);
	bool EditLayerProperties(bool canUndo = true);
	void CommitTransform();

private:
	CFotografixDoc *pDoc;

	// Scroll adjustment
	int dx, dy;

	// Current view size
	int cx, cy;

	// Zoom factor
	float zoom;

	TCHAR sizeText[30];
	TCHAR zoomText[20];

	// Zoomed image dimensions
	int zx, zy;

	// Selected layer
	int selLayer;
	FGXLayer *layer;

	// Drag information
	bool dragging;
	int curTool;
	CRect dragRect;
	CRect dragRectT;
	CRect moveRect;
	CPoint clonePt;
	static int saveTool;

	afx_msg void OnUpdateStatusLayer(CCmdUI *pCmdUI);
	afx_msg void OnUpdateStatusSize(CCmdUI *pCmdUI);
	afx_msg void OnUpdateStatusZoom(CCmdUI *pCmdUI);

	friend class FGXScript;
};

#ifndef _DEBUG  // debug version in FotografixView.cpp
inline CFotografixDoc* CFotografixView::GetDocument() const
   { return reinterpret_cast<CFotografixDoc*>(m_pDocument); }
#endif

