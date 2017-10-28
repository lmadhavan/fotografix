// FotografixView.cpp : implementation of the CFotografixView class
//

#include "stdafx.h"
#include <cmath>

#include "FotografixView.h"

#include "CanvasDialog.h"
#include "InputDialog.h"
#include "FillDialog.h"
#include "SizeDialog.h"
#include "LayerDialog.h"
#include "TextDialog.h"
#include "RulerDialog.h"

#include "FGXGradient.h"
#include "FGXScript.h"

#include "Language.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif

extern HCURSOR toolCursor[];

const int lastFilter = ID_FILTER_EDGES_DIAGONAL;
const int lastAdjustmentLayer = ID_LAYER_LEVELS;

// CFotografixView

int CFotografixView::saveTool;

IMPLEMENT_DYNCREATE(CFotografixView, CScrollView)

BEGIN_MESSAGE_MAP(CFotografixView, CScrollView)
	// Standard printing commands
	ON_COMMAND(ID_FILE_PRINT, &CScrollView::OnFilePrint)
	ON_COMMAND(ID_FILE_PRINT_DIRECT, &CScrollView::OnFilePrint)
	ON_COMMAND(ID_FILE_PRINT_PREVIEW, &CScrollView::OnFilePrintPreview)
	ON_WM_SIZE()
	ON_WM_LBUTTONDOWN()
	ON_WM_MOUSEMOVE()
	ON_WM_LBUTTONUP()
	ON_WM_TIMER()
	ON_COMMAND(ID_EDIT_UNDO, &CFotografixView::OnEditUndo)
	ON_UPDATE_COMMAND_UI(ID_EDIT_UNDO, &CFotografixView::OnUpdateEditUndo)
	ON_COMMAND(ID_EDIT_REDO, &CFotografixView::OnEditRedo)
	ON_UPDATE_COMMAND_UI(ID_EDIT_REDO, &CFotografixView::OnUpdateEditRedo)
	ON_UPDATE_COMMAND_UI(ID_WINDOW_NEW, &CFotografixView::OnUpdateWindowNew)
	ON_COMMAND(ID_EDIT_PURGEUNDO, &CFotografixView::OnEditPurgeundo)
	ON_UPDATE_COMMAND_UI(ID_EDIT_PURGEUNDO, &CFotografixView::OnUpdateEditPurgeundo)
	ON_COMMAND(ID_EDIT_PURGECLIPBOARD, &CFotografixView::OnEditPurgeclipboard)
	ON_UPDATE_COMMAND_UI(ID_EDIT_PURGECLIPBOARD, &CFotografixView::OnUpdateEditPaste)
	ON_COMMAND(ID_EDIT_PURGEALL, &CFotografixView::OnEditPurgeall)
	ON_UPDATE_COMMAND_UI(ID_EDIT_PURGEALL, &CFotografixView::OnUpdateEditPurgeall)
	ON_COMMAND(ID_SELECT_ALL, &CFotografixView::OnSelectAll)
	ON_COMMAND(ID_SELECT_DESELECT, &CFotografixView::OnSelectDeselect)
	ON_COMMAND(ID_SELECT_INVERSE, &CFotografixView::OnSelectInverse)
	ON_UPDATE_COMMAND_UI(ID_SELECT_DESELECT, &CFotografixView::OnUpdateSelectDeselect)
	ON_UPDATE_COMMAND_UI(ID_MODIFY_EXPAND, &CFotografixView::OnUpdateSelectDeselect)
	ON_UPDATE_COMMAND_UI(ID_MODIFY_CONTRACT, &CFotografixView::OnUpdateSelectDeselect)
	ON_UPDATE_COMMAND_UI(ID_MODIFY_FEATHER, &CFotografixView::OnUpdateSelectDeselect)
	ON_COMMAND(ID_EDIT_COPY, &CFotografixView::OnEditCopy)
	ON_UPDATE_COMMAND_UI(ID_EDIT_COPY, &CFotografixView::OnUpdateEditCopy)
	ON_COMMAND(ID_EDIT_PASTE, &CFotografixView::OnEditPaste)
	ON_UPDATE_COMMAND_UI(ID_EDIT_PASTE, &CFotografixView::OnUpdateEditPaste)
	ON_COMMAND(ID_EDIT_CLEAR, &CFotografixView::OnEditClear)
	ON_UPDATE_COMMAND_UI(ID_EDIT_CLEAR, &CFotografixView::OnUpdateEditCopy)
	ON_COMMAND(ID_IMAGE_CROP, &CFotografixView::OnImageCrop)
	ON_UPDATE_COMMAND_UI(ID_IMAGE_CROP, &CFotografixView::OnUpdateEditCopy)
	ON_COMMAND(ID_EDIT_CUT, &CFotografixView::OnEditCut)
	ON_UPDATE_COMMAND_UI(ID_EDIT_CUT, &CFotografixView::OnUpdateEditCopy)
	ON_COMMAND(ID_IMAGE_REVEALALL, &CFotografixView::OnImageRevealall)
	ON_COMMAND(ID_VIEW_ZOOMIN, &CFotografixView::OnViewZoomin)
	ON_COMMAND(ID_VIEW_ZOOMOUT, &CFotografixView::OnViewZoomout)
	ON_COMMAND(ID_VIEW_SCALETOFIT, &CFotografixView::OnViewScaletofit)
	ON_COMMAND(ID_VIEW_ACTUALSIZE, &CFotografixView::OnViewActualsize)
	ON_COMMAND(ID_IMAGE_CANVASSIZE, &CFotografixView::OnImageCanvassize)
	ON_WM_SETCURSOR()
	ON_COMMAND_RANGE(ID_ADJUSTMENTS_BRIGHTNESS, lastFilter, &CFotografixView::OnAdjustment)
	ON_MESSAGE(WM_APP, OnApp)
	ON_COMMAND(ID_LAYER_NEW, &CFotografixView::OnLayerNew)
	ON_COMMAND(ID_NEW_LAYERVIACOPY, &CFotografixView::OnNewLayerviacopy)
	ON_UPDATE_COMMAND_UI(ID_NEW_LAYERVIACOPY, &CFotografixView::OnUpdateNewLayerviacopy)
	ON_COMMAND(ID_NEW_LAYERVIACUT, &CFotografixView::OnNewLayerviacut)
	ON_UPDATE_COMMAND_UI(ID_NEW_LAYERVIACUT, &CFotografixView::OnUpdateNewLayerviacopy)
	ON_COMMAND(ID_LAYER_DUPLICATELAYER, &CFotografixView::OnLayerDuplicatelayer)
	ON_UPDATE_COMMAND_UI(ID_ADJUSTMENTS_COLOURBALANCE, &CFotografixView::OnUpdateAdjustments)
	ON_UPDATE_COMMAND_UI(ID_ADJUSTMENTS_DESATURATE, &CFotografixView::OnUpdateAdjustments)
	ON_UPDATE_COMMAND_UI(ID_ADJUSTMENTS_BLACKWHITE, &CFotografixView::OnUpdateAdjustments)
	ON_UPDATE_COMMAND_UI(ID_ADJUSTMENTS_GRADIENTMAP, &CFotografixView::OnUpdateAdjustments)
	ON_UPDATE_COMMAND_UI(ID_ADJUSTMENTS_THRESHOLD, &CFotografixView::OnUpdateAdjustments)
	ON_UPDATE_COMMAND_UI(ID_FILTER_NIGHTVISION, &CFotografixView::OnUpdateAdjustments)
	ON_COMMAND(ID_LAYER_PROPERTIES, &CFotografixView::OnLayerProperties)
	ON_COMMAND_RANGE(ID_LAYER_BRIGHTNESS, lastAdjustmentLayer, &CFotografixView::OnNewAdjustmentLayer)
	ON_COMMAND(ID_LAYER_DELETE, &CFotografixView::OnLayerDelete)
	ON_UPDATE_COMMAND_UI(ID_LAYER_DELETE, &CFotografixView::OnUpdateLayerDelete)
	ON_COMMAND(ID_LAYER_RENAME, &CFotografixView::OnLayerRename)
	ON_UPDATE_COMMAND_UI(ID_INDICATOR_LAYER, &CFotografixView::OnUpdateStatusLayer)
	ON_UPDATE_COMMAND_UI(ID_INDICATOR_SIZE, &CFotografixView::OnUpdateStatusSize)
	ON_UPDATE_COMMAND_UI(ID_INDICATOR_ZOOM, &CFotografixView::OnUpdateStatusZoom)
	ON_COMMAND(ID_LAYER_ADDLAYERMASK, &CFotografixView::OnLayerAddlayermask)
	ON_UPDATE_COMMAND_UI(ID_LAYER_ADDLAYERMASK, &CFotografixView::OnUpdateLayerAddlayermask)
	ON_COMMAND(ID_LAYER_REMOVELAYERMASK, &CFotografixView::OnLayerRemovelayermask)
	ON_UPDATE_COMMAND_UI(ID_LAYER_REMOVELAYERMASK, &CFotografixView::OnUpdateLayerRemovelayermask)
	ON_COMMAND(ID_LAYER_ENABLELAYERMASK, &CFotografixView::OnLayerEnablelayermask)
	ON_UPDATE_COMMAND_UI(ID_LAYER_ENABLELAYERMASK, &CFotografixView::OnUpdateLayerEnablelayermask)
	ON_COMMAND(ID_EDIT_FILL, &CFotografixView::OnEditFill)
	ON_COMMAND(ID_IMAGE_IMAGESIZE, &CFotografixView::OnImageImagesize)
	ON_COMMAND(ID_FILE_SAVE, &CFotografixView::OnFileSave)
	ON_COMMAND(ID_FILE_SAVE_AS, &CFotografixView::OnFileSaveAs)
	ON_COMMAND(ID_FLIP_HORIZONTAL, &CFotografixView::OnFlipHorizontal)
	ON_COMMAND(ID_FLIP_VERTICAL, &CFotografixView::OnFlipVertical)
	ON_COMMAND(ID_LAYER_MOVEUP, &CFotografixView::OnLayerMoveup)
	ON_COMMAND(ID_LAYER_MOVEDOWN, &CFotografixView::OnLayerMovedown)
	ON_COMMAND(ID_SELECT_LAYERTRANSPARENCY, &CFotografixView::OnSelectLayertransparency)
	ON_COMMAND(ID_LAYER_RASTERIZE, &CFotografixView::OnLayerRasterize)
	ON_UPDATE_COMMAND_UI(ID_LAYER_RASTERIZE, &CFotografixView::OnUpdateLayerRasterize)
	ON_COMMAND(ID_MODIFY_EXPAND, &CFotografixView::OnModifyExpand)
	ON_COMMAND(ID_MODIFY_CONTRACT, &CFotografixView::OnModifyContract)
	ON_COMMAND(ID_MODIFY_FEATHER, &CFotografixView::OnModifyFeather)
	ON_COMMAND(ID_EDIT_FILL_FG, &CFotografixView::OnEditFillFg)
	ON_COMMAND(ID_EDIT_FILL_BG, &CFotografixView::OnEditFillBg)
	ON_COMMAND(ID_ROTATE_90, &CFotografixView::OnRotate90)
	ON_COMMAND(ID_ROTATE_180, &CFotografixView::OnRotate180)
	ON_COMMAND(ID_ROTATE_270, &CFotografixView::OnRotate270)
	ON_COMMAND(ID_ROTATE_OTHER, &CFotografixView::OnRotateOther)
	ON_WM_RBUTTONDOWN()
	ON_WM_HSCROLL()
	ON_WM_VSCROLL()
	ON_COMMAND(ID_LAYER_MERGEDOWN, &CFotografixView::OnLayerMergedown)
	ON_UPDATE_COMMAND_UI(ID_LAYER_MERGEDOWN, &CFotografixView::OnUpdateLayerMergedown)
	ON_COMMAND(ID_LAYER_FLATTENIMAGE, &CFotografixView::OnLayerFlattenimage)
	ON_UPDATE_COMMAND_UI(ID_LAYER_FLATTENIMAGE, &CFotografixView::OnUpdateLayerFlattenimage)
	ON_COMMAND(ID_EDIT_PASTEEXTERNAL, &CFotografixView::OnEditPasteexternal)
	ON_UPDATE_COMMAND_UI(ID_EDIT_PASTEEXTERNAL, &CFotografixView::OnUpdateEditPasteexternal)
	ON_WM_MOUSEWHEEL()
	ON_WM_MBUTTONDOWN()
	ON_WM_MBUTTONUP()
	ON_WM_RBUTTONUP()
	END_MESSAGE_MAP()

// CFotografixView construction/destruction

CFotografixView::CFotografixView() : pDoc(NULL), dragging(false), selLayer(0), layer(NULL)
{
	cx = cy = 0;
	zx = zy = 0;
	clonePt.x = clonePt.y = 0;
}

CFotografixView::~CFotografixView()
{
}

BOOL CFotografixView::PreCreateWindow(CREATESTRUCT& cs)
{
	cs.lpszClass = ::AfxRegisterWndClass(0, ::LoadCursor(NULL, IDC_ARROW), reinterpret_cast<HBRUSH>(::GetStockObject(LTGRAY_BRUSH)));

	return CScrollView::PreCreateWindow(cs);
}

// CFotografixView drawing

void CFotografixView::OnDraw(CDC *pDC)
{
	if (pDoc != NULL)
		if (pDC->IsPrinting()) {
			int px = pDC->GetDeviceCaps(LOGPIXELSX) * pDoc->image.GetWidth() / pDoc->image.GetResolution(),
				py = pDC->GetDeviceCaps(LOGPIXELSY) * pDoc->image.GetHeight() / pDoc->image.GetResolution();

			pDC->SetStretchBltMode(COLORONCOLOR);
			pDoc->bitmap.Draw(pDC, 0, 0, px, py);
		} else {
			pDC->SelectStockObject(BLACK_PEN);
			pDC->SelectStockObject(NULL_BRUSH);
			pDC->Rectangle(dx - 1, dy - 1, dx + zx + 1, dy + zy + 1);

			pDC->SetStretchBltMode(COLORONCOLOR);
			pDoc->bitmap.Draw(pDC, dx, dy, zx, zy);

			//if (globals.curTool == ID_TOOL_CLONE) {
			//	CPoint pt;
			//	UntransformPoint(pt = clonePt);

			//	pDC->SetROP2(R2_NOT);
			//	pDC->MoveTo(pt.x - 5, pt.y); pDC->LineTo(pt.x + 5, pt.y);
			//	pDC->MoveTo(pt.x, pt.y - 5); pDC->LineTo(pt.x, pt.y + 5);
			//}

			if (pDoc->tActive == true)
				pDoc->tTracker.Draw(pDC);
		}
}

void CFotografixView::OnInitialUpdate()
{
	CScrollView::OnInitialUpdate();

	pDoc = GetDocument();
	SelectLayer(0);

	int w = pDoc->image.GetWidth() + 50,
		h = pDoc->image.GetHeight() + 50;

	GetParent()->SetWindowPos(NULL, 0, 0, min(800, w), min(600, h), SWP_NOMOVE | SWP_NOZORDER);

	if (w > 800 || h > 600)
		OnViewScaletofit();
	else
		OnViewActualsize();

	pDoc->Redraw(true);

	SetTimer(1, 100, NULL);
}

void CFotografixView::OnUpdate(CView* /*pSender*/, LPARAM lHint, CObject* /*pHint*/)
{
	if (lHint == true) {
		SetZoomFactor(zoom);

		float w = pDoc->image.GetWidth(),
			  h = pDoc->image.GetHeight();
		int res = pDoc->image.GetResolution(),
			unit = pDoc->image.GetUnit();

		if (unit > 0) {
			w = w / res;
			h = h / res;
		}

		switch (unit) {
			case 0:
				_stprintf(sizeText, TEXT("%.0f x %.0f %s"), w, h, LangItem(pixels));
				break;

			case 1:
				_stprintf(sizeText, TEXT("%.2f x %.2f %s (%d %s)"), w, h, LangItem(inches_short), res, LangItem(pixels_per_inch));
				break;

			case 2:
				_stprintf(sizeText, TEXT("%.2f x %.2f %s (%d %s)"), w, h, LangItem(centimetres_short), res, LangItem(centimetres_per_inch));
				break;
		}
	} else
		Invalidate(false);
}

// CFotografixView printing

BOOL CFotografixView::OnPreparePrinting(CPrintInfo* pInfo)
{
	// default preparation
	return DoPreparePrinting(pInfo);
}

void CFotografixView::OnBeginPrinting(CDC* /*pDC*/, CPrintInfo* /*pInfo*/)
{
	// TODO: add extra initialization before printing
}

void CFotografixView::OnEndPrinting(CDC* /*pDC*/, CPrintInfo* /*pInfo*/)
{
	// TODO: add cleanup after printing
}


// CFotografixView diagnostics

#ifdef _DEBUG
void CFotografixView::AssertValid() const
{
	CScrollView::AssertValid();
}

void CFotografixView::Dump(CDumpContext& dc) const
{
	CScrollView::Dump(dc);
}

CFotografixDoc* CFotografixView::GetDocument() const // non-debug version is inline
{
	ASSERT(m_pDocument->IsKindOf(RUNTIME_CLASS(CFotografixDoc)));
	return (CFotografixDoc*)m_pDocument;
}
#endif //_DEBUG

void CFotografixView::SetZoomFactor(float factor) {
	if (pDoc != NULL) {
		zoom = factor;
		_stprintf(zoomText, TEXT("%.2f%%"), zoom * 100.0f);

		zx = pDoc->image.GetWidth() * zoom;
		zy = pDoc->image.GetHeight() * zoom;
		SetScrollSizes(MM_TEXT, CSize(zx, zy));

		ComputeSizes();

		Invalidate();
	}
}

void CFotografixView::ComputeSizes() {
	RECT rect;
	GetClientRect(&rect);

	cx = rect.right + 1;
	cy = rect.bottom + 1;

	dx = (cx > zx) ? (cx - zx) / 2 : 0;
	dy = (cy > zy) ? (cy - zy) / 2 : 0;

	if (pDoc != NULL && pDoc->tActive == true) CalcHandles();
}

// CFotografixView message handlers

void CFotografixView::OnSize(UINT nType, int cx, int cy)
{
	ComputeSizes();

	CScrollView::OnSize(nType, cx, cy);
}

void CFotografixView::DrawTool(CDC *pDC) {
	switch (curTool) {
	case ID_TOOL_RSELECT:
	//case ID_TOOL_TEXT:
		pDC->Rectangle(dragRect);
		break;

	case ID_TOOL_ESELECT:
		pDC->Ellipse(dragRect);
		break;

	case ID_TOOL_RULER:
	case ID_TOOL_GRADIENT:
		pDC->MoveTo(dragRect.TopLeft());
		pDC->LineTo(dragRect.BottomRight());
		break;

	case ID_TOOL_MOVE:
		{
			CRect rect = moveRect;
			rect.OffsetRect(dragRect.BottomRight() - dragRect.TopLeft());
			pDC->Rectangle(rect);
		}
		break;
	}
}

void CFotografixView::OnLButtonDown(UINT nFlags, CPoint point)
{
	if (dragging == false) {
		// Redirect transform to tracker
		if (pDoc->tActive == true) {
			KillTimer(1);
			if (pDoc->tTracker.Track(this, point) == true)
				CommitTransform();
			SetTimer(1, 100, NULL);
			return;
		}

		// Start dragging
		dragging = true;
		curTool = globals.curTool;
		SetCapture();

		KillTimer(1);

		dragRect.TopLeft() = dragRect.BottomRight() = point;

		dragRectT = dragRect;
		TransformRect(dragRectT);

		switch (curTool) {
		case ID_TOOL_DROPPER:
			if (layer->GetPosition().PtInRect(dragRectT.BottomRight()))
				globals.fgColor.SetColor(layer->PixelAt(dragRectT.BottomRight().x, dragRectT.BottomRight().y));
			break;

		case ID_TOOL_BRUSH:
			if (layer->GetType() == LayerText) {
				dragging = false;
				ReleaseCapture();

				MessageBox(LangMessage(ErrorRasterize), NULL, MB_ICONWARNING | MB_OK);
			} else {
				layer->Clone(pDoc->GetUndoLayer(selLayer, LangItem(Brush)));
				pDoc->SetModifiedFlag(true);
				Paint(globals.brush, *layer, dragRectT.BottomRight());
			}
			break;

		case ID_TOOL_ERASER:
			if (layer->GetType() == LayerText) {
				dragging = false;
				ReleaseCapture();

				MessageBox(LangMessage(ErrorRasterize), NULL, MB_ICONWARNING | MB_OK);
			} else {
				layer->Clone(pDoc->GetUndoLayer(selLayer, LangItem(Eraser)));
				pDoc->SetModifiedFlag(true);
				Erase(globals.brush, *layer, dragRectT.BottomRight());
			}
			break;

		case ID_TOOL_CLONE:
			if (layer->GetType() == LayerText) {
				dragging = false;
				ReleaseCapture();

				MessageBox(LangMessage(ErrorRasterize), NULL, MB_ICONWARNING | MB_OK);
			} else if ((GetKeyState(VK_MENU) & 0x8000) > 0) {
				// Set source point
				TransformPoint(clonePt = point);

				dragging = false;
				ReleaseCapture();
			} else {
				layer->Clone(pDoc->GetUndoLayer(selLayer, LangItem(Clone)));
				pDoc->SetModifiedFlag(true);
				Clone(globals.brush, *layer, dragRectT.BottomRight(), clonePt + (dragRectT.BottomRight() - dragRectT.TopLeft()));
			}
			break;

		case ID_TOOL_MOVE:
			moveRect = pDoc->selection.GetPosition();
			if (moveRect.IsRectEmpty()) moveRect = layer->GetPosition();
			UntransformRect(moveRect);
			break;

		case ID_TOOL_HAND:
			SetCursor(toolCursor[15]);
			break;
		}
	}
}

void CFotografixView::OnMouseMove(UINT nFlags, CPoint point)
{
	if (dragging == true) {
		CDC *pDC = GetDC();
		pDC->SetROP2(R2_NOT);
		pDC->SelectStockObject(NULL_BRUSH);

		// Erase previous lines
		DrawTool(pDC);

		dragRect.BottomRight() = point;

		// Check selection options
		if (curTool <= ID_TOOL_ESELECT) {
			if (globals.selStyle == 1)
				dragRect.bottom = dragRect.top + dragRect.Width() * globals.selH / globals.selW;
			else if (globals.selStyle == 2) {
				dragRect.left = dragRect.right - globals.selW * zoom;
				dragRect.top = dragRect.bottom - globals.selH * zoom;
			} else if (nFlags & MK_SHIFT)
				dragRect.bottom = dragRect.top + dragRect.Width();
		} else if ((nFlags & MK_SHIFT) && curTool >= ID_TOOL_MOVE && curTool <= ID_TOOL_GRADIENT) {
			int dx = abs(dragRect.Width()),
				dy = abs(dragRect.Height());

			if (dx > 2 * dy)
				dragRect.bottom = dragRect.top;
			else if (dy > 2 * dx)
				dragRect.right = dragRect.left;
			else
				dragRect.bottom = dragRect.top + ((dragRect.Width() * dragRect.Height() > 0) ? dragRect.Width() : -dragRect.Width());
		}

		dragRectT.BottomRight() = dragRect.BottomRight();
		TransformPoint(dragRectT.BottomRight());

		// Draw new lines
		DrawTool(pDC);

		switch (curTool) {
		case ID_TOOL_DROPPER:
			if (layer->GetPosition().PtInRect(dragRectT.BottomRight()))
				globals.fgColor.SetColor(layer->PixelAt(dragRectT.BottomRight().x, dragRectT.BottomRight().y));
			break;

		case ID_TOOL_HAND:
			ScrollToPosition(GetScrollPosition() - (dragRectT.BottomRight() - dragRectT.TopLeft()));
			break;

		case ID_TOOL_BRUSH:
			Paint(globals.brush, *layer, dragRectT.BottomRight());
			break;

		case ID_TOOL_ERASER:
			Erase(globals.brush, *layer, dragRectT.BottomRight());
			break;

		case ID_TOOL_CLONE:
			Clone(globals.brush, *layer, dragRectT.BottomRight(), clonePt + (dragRectT.BottomRight() - dragRectT.TopLeft()));
			break;
		}

		ReleaseDC(pDC);
	}
}

void CFotografixView::OnLButtonUp(UINT nFlags, CPoint point)
{
	if (dragging == true) {
		// Stop dragging
		ReleaseCapture();
		dragging = false;

		CDC *pDC = GetDC();
		pDC->SetROP2(R2_NOT);
		pDC->SelectStockObject(NULL_BRUSH);

		// Erase lines
		DrawTool(pDC);

		ReleaseDC(pDC);

		// Transform the coordinates
		TransformRect(dragRect);
		dragRect.NormalizeRect();

		SelectType selType = (nFlags & MK_CONTROL) ? SelectAdd : ((::GetAsyncKeyState(VK_MENU) & 0x8000) ? SelectSubtract : SelectNormal);

		switch (curTool) {
		case ID_TOOL_RSELECT:
			pDoc->CleanSelection();
			pDoc->selection.SelectRectangle(pDoc->GetUndoSelection(LangItem(RSelect)), dragRect & pDoc->bounds, selType);
			pDoc->PrepareSelection();
			break;

		case ID_TOOL_ESELECT:
			pDoc->CleanSelection();
			pDoc->selection.SelectEllipse(pDoc->GetUndoSelection(LangItem(ESelect)), dragRect & pDoc->bounds, selType);
			pDoc->PrepareSelection();
			break;

		case ID_TOOL_WAND:
			pDoc->CleanSelection();
			{
				FGXLayer temp;
				layer->ExpandTo(temp, pDoc->bounds);
				layer->Swap(temp);

				if (globals.wandContiguous)
					pDoc->selection.SelectWand(pDoc->GetUndoSelection(LangItem(MagicWand)), temp, dragRect.BottomRight(), globals.wandTolerance, selType);
				else
					pDoc->selection.SelectRange(pDoc->GetUndoSelection(LangItem(MagicWand)), temp, dragRect.BottomRight(), globals.wandTolerance, selType);
			}
			pDoc->PrepareSelection();
			break;

		case ID_TOOL_GRADIENT:
			if (layer->GetType() == LayerText)
				MessageBox(LangMessage(ErrorRasterize), NULL, MB_ICONWARNING | MB_OK);
			else {
				pDoc->SaveUndoLayer(selLayer, LangItem(Gradient));

				FGXGradient g(globals.gradType, pDoc->bounds, dragRectT.TopLeft(), dragRectT.BottomRight());

				if (pDoc->selection.GetPosition().IsRectEmpty() == false) {
					if (globals.gradColor == 0)
						g.Render(*layer, pDoc->selection, FGXColor(globals.fgColor.GetColor(), 255), FGXColor(globals.bgColor.GetColor(), 255), pDoc->channelMask);
					else
						g.Render(*layer, pDoc->selection, FGXColor(globals.fgColor.GetColor(), 255));

					pDoc->RedrawLayerSelection(selLayer);
				} else {
					if (globals.gradColor == 0)
						g.Render(*layer, FGXColor(globals.fgColor.GetColor(), 255), FGXColor(globals.bgColor.GetColor(), 255), pDoc->channelMask);
					else
						g.Render(*layer, FGXColor(globals.fgColor.GetColor(), 255));

					pDoc->RedrawLayer(selLayer);
				}
			}
			break;

		case ID_TOOL_MOVE:
			{
				CPoint delta = dragRectT.BottomRight() - dragRectT.TopLeft();

				if ((::GetKeyState(VK_MENU) & 0x8000) > 0 && pDoc->selection.GetPosition().IsRectEmpty() == false) {
					// Move selection

					CRect rect = pDoc->selection.GetPosition();
					rect.OffsetRect(delta);

					pDoc->CleanSelection();
					pDoc->SaveUndoMoveSelection();
					pDoc->selection.MoveTo(rect.left, rect.top);
					pDoc->PrepareSelection();
					Invalidate();
				} else if (pDoc->selection.GetPosition().IsRectEmpty() == true) {
					// Move layer

					if (layer->GetType() == LayerAdjust)
						MessageBox(LangMessage(ErrorAdjMove), NULL, MB_ICONWARNING | MB_OK);
					else {
						pDoc->SaveUndoMoveLayer(selLayer);

						const CRect &rect = layer->GetPosition();
						layer->MoveTo(rect.left + delta.x, rect.top + delta.y);

						pDoc->Redraw();
						Invalidate();
					}
				} else {
					// Move selected region

					if (layer->GetType() == LayerText) {
						MessageBox(LangMessage(ErrorRasterize), NULL, MB_ICONWARNING | MB_OK);
						break;
					}

					CRect rect = pDoc->selection.GetPosition();
					FGXLayer temp(rect);
					layer->CopySelection(temp, pDoc->selection, pDoc->channelMask);

					bool copy = (nFlags & MK_CONTROL) != 0;

					rect.OffsetRect(delta);
					temp.MoveTo(rect.left, rect.top);
					layer->ExpandTo(pDoc->GetUndoLayerSelection(selLayer, copy ? LangItem(Copy) : LangItem(Move)), rect | layer->GetPosition());
					if (copy == false) layer->Clear(FGXLayer(), pDoc->selection, FGXColor(globals.bgColor.GetColor(), 0), pDoc->channelMask);
					temp.AlphaRender(*layer, temp.GetPosition(), pDoc->channelMask);

					pDoc->selection.MoveTo(rect.left, rect.top);
					pDoc->Redraw();
					Invalidate();
				}
			}
			pDoc->SetModifiedFlag(true);
			break;

		case ID_TOOL_FILL:
			if (layer->GetType() == LayerText)
				MessageBox(LangMessage(ErrorRasterize), NULL, MB_ICONWARNING | MB_OK);
			else {
				FGXSelection selection;

				FGXLayer temp;
				layer->ExpandTo(temp, pDoc->bounds);
				layer->Swap(temp);

				if (globals.wandContiguous)
					selection.SelectWand(FGXChannel(), temp, dragRect.BottomRight(), globals.wandTolerance);
				else
					selection.SelectRange(FGXChannel(), temp, dragRect.BottomRight(), globals.wandTolerance);

				pDoc->SaveUndoLayer(selLayer, LangItem(Fill));
				layer->EnsureRect(selection.GetPosition());
				layer->Clear(FGXLayer(), selection, FGXColor(globals.fgColor.GetColor(), 255), pDoc->channelMask);
				pDoc->RedrawLayer(selLayer);
			}
			pDoc->SetModifiedFlag(true);
			break;

		case ID_TOOL_TEXT:
			{
				int curSel = selLayer;

				FGXLayer *layer = new FGXLayer(LayerText);
				layer->MoveTo(dragRectT.left, dragRectT.top);
				layer->a1 = globals.textSize;
				layer->a2 = globals.textColor;
				layer->a3 = globals.textStyle;
				layer->a4 = globals.textFace;
				SelectLayer(pDoc->image.AddLayer(layer));

				if (EditLayerProperties(false) == true) {
					globals.textSize = layer->a1;
					globals.textColor = layer->a2;
					globals.textStyle = layer->a3;
					globals.textFace = layer->a4;

					layer->SetName(layer->a5.Left(255));
					pDoc->AddUndoNewLayer(selLayer, LangItem(Text));
					OnApp(0, 0);
				} else {
					pDoc->image.DeleteLayer(selLayer);
					SelectLayer(curSel);
				}
			}
			break;

		case ID_TOOL_ZOOM:
			TransformPoint(point);
			if ((GetKeyState(VK_MENU) & 0x8000) > 0)
				OnViewZoomout();
			else
				OnViewZoomin();
			if (dx == 0 && dy == 0)
				ScrollToPosition(CPoint(point.x * zoom - cx / 2, point.y * zoom - cy / 2));
			break;

		case ID_TOOL_RULER:
			{
				int dx = dragRect.right - dragRect.left,
					dy = dragRect.bottom - dragRect.top;

				float distance = sqrt(float(dx*dx + dy*dy)),
					  angle = atan2(float(dy), float(dx));

				CRulerDialog *dlg = new CRulerDialog;
				dlg->point1.Format(TEXT("(%d, %d)"), dragRect.left, dragRect.top);
				dlg->point2.Format(TEXT("(%d, %d)"), dragRect.right, dragRect.bottom);
				dlg->distance.Format(TEXT("%.2f %s"), distance, LangItem(pixels));
				dlg->angle.Format(TEXT("%.2f\xb0\n(%.2f %s)"), angle * 180 / 3.14159, angle, LangItem(radians));
				dlg->Create(IDD_RULER, this);
				dlg->ShowWindow(SW_SHOW);
			}
			break;
		}

		SetTimer(1, 100, NULL);
	}
}

void CFotografixView::OnMButtonDown(UINT nFlags, CPoint point)
{
	saveTool = globals.curTool;
	globals.curTool = ID_TOOL_HAND;
	OnLButtonDown(nFlags, point);
}

void CFotografixView::OnMButtonUp(UINT nFlags, CPoint point)
{
	OnLButtonUp(nFlags, point);
	globals.curTool = saveTool;
}

void CFotografixView::OnRButtonUp(UINT nFlags, CPoint point)
{
	if (curTool == ID_TOOL_ZOOM) {
		TransformPoint(point);
		if ((GetKeyState(VK_MENU) & 0x8000) > 0)
			OnViewZoomin();
		else
			OnViewZoomout();
		if (dx == 0 && dy == 0)
			ScrollToPosition(CPoint(point.x * zoom - cx / 2, point.y * zoom - cy / 2));
	}
}

void CFotografixView::OnRButtonDown(UINT nFlags, CPoint point)
{
	if (pDoc->tActive == true && pDoc->tTracker.HitTest(point) == CRectTracker::hitMiddle) {
		CMenu menu;
		menu.LoadMenu(IDR_POPUP);

		ClientToScreen(&point);

		CMenu *popup = menu.GetSubMenu(1);
		TranslateMenu(popup);

		switch (popup->TrackPopupMenu(TPM_LEFTALIGN | TPM_RIGHTBUTTON | TPM_RETURNCMD, point.x, point.y, this)) {
		case 0:
			return;

		case ID_FLIP_HORIZONTAL:
			pDoc->SaveUndoLayer(selLayer, LangItem(Transform));
			layer->Flip(true, false);
			pDoc->RedrawLayer(selLayer);
			break;

		case ID_FLIP_VERTICAL:
			pDoc->SaveUndoLayer(selLayer, LangItem(Transform));
			layer->Flip(false, true);
			pDoc->RedrawLayer(selLayer);
			break;

		case ID_ROTATE_90:
			layer->Rotate(pDoc->GetUndoLayer(selLayer, LangItem(Transform)), 90);
			pDoc->Redraw();
			break;

		case ID_ROTATE_180:
			layer->Rotate(pDoc->GetUndoLayer(selLayer, LangItem(Transform)), 180);
			pDoc->Redraw();
			break;

		case ID_ROTATE_270:
			layer->Rotate(pDoc->GetUndoLayer(selLayer, LangItem(Transform)), 270);
			pDoc->Redraw();
			break;

		case ID_ROTATE_OTHER:
			{
				CInputDialog dlg;

				dlg.title = LangItem(Rotate);
				dlg.prompt = LangItem(Angle);
				dlg.value = TEXT("45");
				dlg.number = true;

				if (dlg.DoModal() == IDOK) {
					layer->Rotate(pDoc->GetUndoLayer(selLayer, LangItem(Transform)), _ttoi(dlg.value));
					layer->Compact();
					pDoc->Redraw();
				}
			}
			break;
		}

		pDoc->SetModifiedFlag(true);
		CalcHandles();
	} else
		CScrollView::OnRButtonDown(nFlags, point);
}

void CFotografixView::OnHScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar)
{
	if (pDoc->tActive == true)
		CalcHandles();

	CScrollView::OnHScroll(nSBCode, nPos, pScrollBar);
}

void CFotografixView::OnVScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar)
{
	if (pDoc->tActive == true)
		CalcHandles();

	CScrollView::OnVScroll(nSBCode, nPos, pScrollBar);
}

void CFotografixView::CommitTransform() {
	if (layer->GetType() == LayerText)
		MessageBox(LangMessage(ErrorRasterize), NULL, MB_ICONWARNING | MB_OK);
	else {
		CRect rect = pDoc->tTracker.m_rect;
		TransformRect(rect);

		bool flipH = rect.Width() < 0,
			 flipV = rect.Height() < 0;

		rect.NormalizeRect();
		layer->ResizeTo(pDoc->GetUndoLayer(selLayer, LangItem(Transform)), rect);
		layer->Flip(flipH, flipV);

		pDoc->SetModifiedFlag(true);
		CalcHandles();
		pDoc->Redraw(true);
	}
}

void CFotografixView::OnTimer(UINT_PTR nIDEvent)
{
	if (nIDEvent == 1) {
		// Animate the selection outline
		if (pDoc->selection.GetPosition().IsRectEmpty() == false) {
			pDoc->selection.Animate(pDoc->bitmap, pDoc->bounds);
			pDoc->UpdateAllViews(NULL);
		}

		// (De)activate transform mode
		if (pDoc->tActive == false && globals.curTool == ID_TOOL_TRANSFORM) {
			// Activate
			pDoc->tActive = true;
			CalcHandles();
		} else if (pDoc->tActive == true && globals.curTool != ID_TOOL_TRANSFORM) {
			// Deactivate
			pDoc->tActive = false;
			pDoc->UpdateAllViews(NULL);
			Invalidate();
		}
	} else
		CScrollView::OnTimer(nIDEvent);
}

void CFotografixView::CalcHandles() {
	pDoc->tTracker.m_rect = layer->GetPosition();
	UntransformRect(pDoc->tTracker.m_rect);

	pDoc->UpdateAllViews(NULL);
	Invalidate();
}

void CFotografixView::OnEditUndo()
{
	int type = pDoc->Undo();

	if (pDoc->tActive == true)
		CalcHandles();

	if (type == UndoNewLayer || type == UndoOrderLayer || type == UndoImage)
		SelectLayer(0);

	if (type == UndoNewLayer || type == UndoDeleteLayer || type == UndoOrderLayer || type == UndoImage || type == UndoRasterize)
		OnApp(0, 0);

	if (type != UndoSelection && type != UndoMoveSelection)
		pDoc->SetModifiedFlag(true);
}

void CFotografixView::OnUpdateEditUndo(CCmdUI *pCmdUI)
{
	if (pDoc->CanUndo()) {
		pCmdUI->Enable();
		pCmdUI->SetText(LangItemParam(UndoAction, pDoc->GetUndoText()) + CString(TEXT("\tCtrl+Z")));
	} else {
		pCmdUI->Enable(false);
		pCmdUI->SetText(LangItem(CantUndo));
	}
}

void CFotografixView::OnEditRedo()
{
	int type = pDoc->Redo();

	if (pDoc->tActive == true)
		CalcHandles();

	if (type == UndoDeleteLayer || type == UndoOrderLayer || type == UndoImage)
		SelectLayer(0);

	if (type == UndoNewLayer || type == UndoDeleteLayer || type == UndoOrderLayer || type == UndoImage || type == UndoRasterize)
		OnApp(0, 0);

	if (type != UndoSelection && type != UndoMoveSelection)
		pDoc->SetModifiedFlag(true);
}

void CFotografixView::OnUpdateEditRedo(CCmdUI *pCmdUI)
{
	if (pDoc->CanRedo()) {
		pCmdUI->Enable();
		pCmdUI->SetText(LangItemParam(RedoAction, pDoc->GetRedoText()) + CString(TEXT("\tShift+Z")));
	} else {
		pCmdUI->Enable(false);
		pCmdUI->SetText(LangItem(CantRedo));
	}
}

void CFotografixView::OnUpdateWindowNew(CCmdUI *pCmdUI)
{
	pCmdUI->SetText(LangItemParam(NewWindow, pDoc->GetTitle()));
}

void CFotografixView::OnEditPurgeundo()
{
	if (MessageBox(LangMessage(WarnNoUndo), NULL, MB_ICONWARNING | MB_YESNO) == IDYES)
		pDoc->PurgeUndo();
}

void CFotografixView::OnEditPurgeclipboard()
{
	if (MessageBox(LangMessage(WarnNoUndo), NULL, MB_ICONWARNING | MB_YESNO) == IDYES)
		globals.clipboard.SetPosition(CRect(0, 0, 0, 0));
}

void CFotografixView::OnUpdateEditPurgeundo(CCmdUI *pCmdUI)
{
	pCmdUI->Enable(pDoc->CanPurgeUndo());
}

void CFotografixView::OnEditPurgeall()
{
	if (MessageBox(LangMessage(WarnNoUndo), NULL, MB_ICONWARNING | MB_YESNO) == IDYES) {
		pDoc->PurgeUndo();
		globals.clipboard.SetPosition(CRect(0, 0, 0, 0));
	}
}

void CFotografixView::OnUpdateEditPurgeall(CCmdUI *pCmdUI)
{
	pCmdUI->Enable(pDoc->CanPurgeUndo() || globals.clipboard.GetPosition().IsRectEmpty() == false);
}

void CFotografixView::OnSelectAll()
{
	pDoc->CleanSelection();
	pDoc->selection.SelectRectangle(pDoc->GetUndoSelection(LangItem(SelectAll)), pDoc->bounds);
	pDoc->PrepareSelection();
}

void CFotografixView::OnSelectDeselect()
{
	pDoc->CleanSelection();
	pDoc->selection.SelectRectangle(pDoc->GetUndoSelection(LangItem(Deselect)), CRect(0, 0, 0, 0));
	pDoc->PrepareSelection();
}

void CFotografixView::OnUpdateSelectDeselect(CCmdUI *pCmdUI)
{
	pCmdUI->Enable(pDoc->selection.GetPosition().IsRectEmpty() == false);
}

void CFotografixView::OnSelectInverse()
{
	pDoc->CleanSelection();
	pDoc->selection.SelectInverse(pDoc->GetUndoSelection(LangItem(InvertSel)), pDoc->bounds);
	pDoc->PrepareSelection();
}

void CFotografixView::OnEditCut()
{
	if (pDoc->selection.GetPosition().IsRectEmpty() == false) {
		if (layer->GetType() == LayerText)
			MessageBox(LangMessage(ErrorRasterize), NULL, MB_ICONWARNING | MB_OK);
		else {
			OnEditCopy();
			OnEditClear();
		}
	}
}

void CFotografixView::OnEditCopy()
{
	if (pDoc->selection.GetPosition().IsRectEmpty() == false) {
		globals.clipboard.SetPosition(pDoc->selection.GetPosition());
		layer->CopySelection(globals.clipboard, pDoc->selection, pDoc->channelMask);
	}
}

void CFotografixView::OnUpdateEditCopy(CCmdUI *pCmdUI)
{
	pCmdUI->Enable(pDoc->selection.GetPosition().IsRectEmpty() == false);
}

void CFotografixView::OnEditPaste()
{
	if (globals.clipboard.GetPosition().IsRectEmpty() == false) {
		FGXLayer *layer = new FGXLayer;
		globals.clipboard.Clone(*layer);

		const CRect &rect = pDoc->selection.GetPosition();
		if (rect.IsRectEmpty())
			layer->MoveTo((pDoc->bounds.Width() - layer->GetPosition().Width()) / 2, (pDoc->bounds.Height() - layer->GetPosition().Height()) / 2);
		else
			layer->MoveTo(rect.left + (rect.Width() - layer->GetPosition().Width()) / 2, rect.top + (rect.Height() - layer->GetPosition().Height()) / 2);

		SelectLayer(pDoc->image.AddLayer(layer));
		pDoc->RedrawLayer(selLayer);
		pDoc->AddUndoNewLayer(selLayer, LangItem(Paste));

		OnApp(0, 0);
		pDoc->SetModifiedFlag(true);
	}
}

void CFotografixView::OnUpdateEditPaste(CCmdUI *pCmdUI)
{
	pCmdUI->Enable(globals.clipboard.GetPosition().IsRectEmpty() == false);
}

void CFotografixView::OnEditPasteexternal()
{
	if (::IsClipboardFormatAvailable(CF_BITMAP) == true && ::OpenClipboard(NULL) == true) {
		Bitmap bitmap((HBITMAP)::GetClipboardData(CF_BITMAP), NULL);
		FGXLayer layer;
		CFotografixDoc::LoadImage_Bitmap(bitmap, layer);

		layer.Swap(globals.clipboard);
		OnEditPaste();
		layer.Swap(globals.clipboard);

		::CloseClipboard();
	}
}

void CFotografixView::OnUpdateEditPasteexternal(CCmdUI *pCmdUI)
{
	pCmdUI->Enable(::IsClipboardFormatAvailable(CF_BITMAP));
}

void CFotografixView::OnEditClear()
{
	if (pDoc->selection.GetPosition().IsRectEmpty() == false) {
		if (layer->GetType() == LayerText)
			MessageBox(LangMessage(ErrorRasterize), NULL, MB_ICONWARNING | MB_OK);
		else {
			layer->Clear(pDoc->GetUndoLayer(selLayer, LangItem(Clear)), pDoc->selection, FGXColor(globals.bgColor.GetColor(), 0), pDoc->channelMask);
			pDoc->RedrawLayerSelection(selLayer);
			pDoc->SetModifiedFlag(true);
		}
	}
}

void CFotografixView::OnImageCrop()
{
	if (pDoc->selection.GetPosition().IsRectEmpty() == false) {
		pDoc->image.Crop(pDoc->GetUndoImage(LangItem(Crop)), pDoc->selection.GetPosition());
		pDoc->selection.SelectRectangle(FGXChannel(), CRect(0, 0, 0, 0));
		pDoc->Redraw(true);
		pDoc->SetModifiedFlag(true);
	}
}

void CFotografixView::OnImageRevealall()
{
	pDoc->SaveUndoRevealAll();
	pDoc->image.RevealAll();
	pDoc->Redraw(true);
	pDoc->SetModifiedFlag(true);
}

void CFotografixView::OnViewZoomin()
{
	SetZoomFactor(zoom * 1.5);
}

void CFotografixView::OnViewZoomout()
{
	SetZoomFactor(zoom / 1.5);
}

void CFotografixView::OnViewScaletofit()
{
	float fx = float(cx - 3) / pDoc->image.GetWidth(),
		  fy = float(cy - 3) / pDoc->image.GetHeight();

	SetZoomFactor(min(fx, fy));
}

void CFotografixView::OnViewActualsize()
{
	SetZoomFactor(1.0);
}

void CFotografixView::OnImageCanvassize()
{
	CCanvasDialog dlg;

	dlg.ow = pDoc->bounds.right;
	dlg.oh = pDoc->bounds.bottom;

	int res = pDoc->image.GetResolution();
	int unit = pDoc->image.GetUnit();

	switch (unit) {
		case 0: dlg.unitName = LangItem(pixels); break;
		case 1: dlg.unitName = LangItem(inches); break;
		case 2: dlg.unitName = LangItem(centimetres); break;
	}

	if (unit > 0) {
		dlg.ow /= res;
		dlg.oh /= res;
	}

	if (dlg.DoModal() == IDOK) {
		if (unit > 0) {
			dlg.w2 *= res;
			dlg.h2 *= res;
		}

		pDoc->SaveUndoCanvasSize(dlg.anchor);
		pDoc->image.ResizeCanvas(dlg.w2, dlg.h2, dlg.anchor);
		pDoc->Redraw(true);
		pDoc->SetModifiedFlag(true);
	}
}

BOOL CFotografixView::OnSetCursor(CWnd* pWnd, UINT nHitTest, UINT message)
{
	if (nHitTest == HTCLIENT) {
		if (pDoc->tActive == true && pDoc->tTracker.SetCursor(pWnd, nHitTest) == true)
			return true;

		SetCursor(globals.curCursor);
		return true;
	} else
		return CScrollView::OnSetCursor(pWnd, nHitTest, message);
}

bool CFotografixView::InitAdjustDialog(CAdjustDialog &dlg, int type) {
	dlg.pDoc = pDoc;
	dlg.layer = selLayer;
	dlg.adjustType = type;
	dlg.channelMask = pDoc->channelMask;

	if (::GetKeyState(VK_RCONTROL) & 0x8000) dlg.preview = false;

	switch (type) {
	case AdjustBrightnessContrast:
		dlg.num = 2;
		dlg.title = LangItem(BrightnessContrast);
		dlg.label[0] = LangItem(Brightness);
		dlg.label[1] = LangItem(Contrast);
		dlg.min[0] = dlg.min[1] = -255;
		dlg.max[0] = dlg.max[1] = 255;
		dlg.def[0] = dlg.def[1] = 0;
		return true;

	case AdjustColorBalance:
		dlg.num = 3;
		dlg.title = LangItem(ColourBalance);
		dlg.label[0] = LangItem(Red);
		dlg.label[1] = LangItem(Green);
		dlg.label[2] = LangItem(Blue);
		dlg.min[0] = dlg.min[1] = dlg.min[2] = -255;
		dlg.max[0] = dlg.max[1] = dlg.max[2] = 255;
		dlg.def[0] = dlg.def[1] = dlg.def[2] = 0;
		return true;

	case AdjustLevels:
		dlg.num = 3;
		dlg.title = LangItem(Levels);
		dlg.label[0] = LangItem(Shadows);
		dlg.label[1] = LangItem(Midtones);
		dlg.label[2] = LangItem(Highlights);
		dlg.min[0] = dlg.min[2] = 0; dlg.min[1] = 75;
		dlg.max[0] = dlg.max[2] = 255; dlg.max[1] = 125;
		dlg.def[0] = 0;
		dlg.def[1] = 100;
		dlg.def[2] = 255;
		return true;

	//case AdjustHueSaturation:
	//	dlg.num = 2;
	//	dlg.title = LangItem(HueSaturation);
	//	dlg.label[0] = LangItem(Hue);
	//	dlg.label[1] = LangItem(Saturation);
	//	dlg.min[0] = dlg.min[1] = -255;
	//	dlg.max[0] = dlg.max[1] = 255;
	//	dlg.def[0] = dlg.def[1] = 0;
	//	return true;

	case AdjustDesaturate:
		dlg.title = LangItem(Desaturate);
		return false;

	case AdjustBlackWhite:
		dlg.num = 3;
		dlg.title = LangItem(BlackWhite);
		dlg.label[0] = LangItem(Red);
		dlg.label[1] = LangItem(Green);
		dlg.label[2] = LangItem(Blue);
		dlg.min[0] = dlg.min[1] = dlg.min[2] = 0;
		dlg.max[0] = dlg.max[1] = dlg.max[2] = 100;
		dlg.def[0] = 30; dlg.def[1] = 59; dlg.def[2] = 11;
		return true;

	case AdjustGradientMap:
		dlg.title = LangItem(GradientMap);
		return false;

	case AdjustInvert:
		dlg.title = LangItem(Invert);
		return false;

	case AdjustPosterize:
		dlg.num = 1;
		dlg.title = LangItem(Posterize);
		dlg.label[0] = LangItem(Level);
		dlg.min[0] = 3;
		dlg.max[0] = 7;
		dlg.def[0] = 3;
		return true;

	case AdjustThreshold:
		dlg.num = 1;
		dlg.title = LangItem(Threshold);
		dlg.label[0] = LangItem(Threshold);
		dlg.min[0] = 0;
		dlg.max[0] = 255;
		dlg.def[0] = 128;
		return true;

	case FilterBlur:
		dlg.num = 1;
		dlg.title = LangItem(Blur);
		dlg.label[0] = LangItem(Radius);
		dlg.min[0] = 1;
		dlg.max[0] = 200;
		dlg.def[0] = 1;
		return true;

	case FilterMotionBlur:
		dlg.num = 1;
		dlg.title = LangItem(MotionBlur);
		dlg.label[0] = LangItem(Distance);
		dlg.min[0] = 1;
		dlg.max[0] = 200;
		dlg.def[0] = 5;
		return true;

	case FilterGaussianBlur:
		dlg.num = 1;
		dlg.title = LangItem(GaussianBlur);
		dlg.label[0] = LangItem(Radius);
		dlg.min[0] = 1;
		dlg.max[0] = 200;
		dlg.def[0] = 1;
		return true;

	case FilterShear:
		dlg.num = 1;
		dlg.title = LangItem(Diffuse);
		dlg.label[0] = LangItem(Radius);
		dlg.min[0] = 2;
		dlg.max[0] = 200;
		dlg.def[0] = 10;
		return true;

	case FilterAddNoise:
		dlg.num = 1;
		dlg.title = LangItem(AddNoise);
		dlg.label[0] = LangItem(Amount);
		dlg.min[0] = 1;
		dlg.max[0] = 100;
		dlg.def[0] = 25;
		return true;

	case FilterOffset:
		dlg.num = 2;
		dlg.title = LangItem(Offset);
		dlg.label[0] = LangItem(Horizontal);
		dlg.label[1] = LangItem(Vertical);
		dlg.min[0] = dlg.min[1] = -32000;
		dlg.max[0] = dlg.max[1] = 32000;
		dlg.def[0] = dlg.def[1] = 0;
		return true;

	case FilterPixelate:
		dlg.num = 1;
		dlg.title = LangItem(Pixelate);
		dlg.label[0] = LangItem(Level);
		dlg.min[0] = 1;
		dlg.max[0] = 31;
		dlg.def[0] = 3;
		return true;

	case FilterHalftone:
		dlg.num = 1;
		dlg.title = LangItem(Halftone);
		dlg.label[0] = LangItem(Radius);
		dlg.min[0] = 1;
		dlg.max[0] = 100;
		dlg.def[0] = 50;
		return true;

	case FilterSharpen:
		dlg.title = LangItem(Sharpen);
		return false;

	case FilterUnsharpMask:
		dlg.num = 1;
		dlg.title = LangItem(UnsharpMask);
		dlg.label[0] = LangItem(Amount);
		dlg.min[0] = 1;
		dlg.max[0] = 100;
		dlg.def[0] = 50;
		return true;

	case FilterEmboss:
		dlg.title = LangItem(Emboss);
		return false;

	case FilterNightVision:
		dlg.title = LangItem(NightVision);
		return false;

	case FilterSolarize:
		dlg.title = LangItem(Solarize);
		return false;

	case FilterEdgesAll:
	case FilterEdgesHorz:
	case FilterEdgesVert:
	case FilterEdgesDiag:
		dlg.title = LangItem(FindEdges);
		return false;

	default:
		return false;
	}
}

void CFotografixView::OnAdjustment(UINT nID)
{
	if (layer->GetType() == LayerText)
		MessageBox(LangMessage(ErrorRasterize), NULL, MB_ICONWARNING | MB_OK);
	else {
		CAdjustDialog dlg;

		if (InitAdjustDialog(dlg, AdjustBrightnessContrast + (nID - ID_ADJUSTMENTS_BRIGHTNESS)))
			dlg.DoModal();
		else {
			if (pDoc->selection.GetPosition().IsRectEmpty()) {
				layer->Adjust(pDoc->GetUndoLayer(selLayer, dlg.title), dlg.adjustType, globals.fgColor.GetColor(), globals.bgColor.GetColor(), 0, pDoc->channelMask);
				pDoc->RedrawLayer(selLayer);
			} else {
				layer->Adjust(pDoc->GetUndoLayer(selLayer, dlg.title), pDoc->selection, dlg.adjustType, globals.fgColor.GetColor(), globals.bgColor.GetColor(), 0, pDoc->channelMask);
				pDoc->RedrawLayerSelection(selLayer);
			}
			pDoc->SetModifiedFlag(true);
		}
	}
}

LRESULT CFotografixView::OnApp(WPARAM wParam, LPARAM lParam) {
	static bool lock = false;

	if (lock == true) return 1;

	switch (wParam) {
	case 0:
		// Refresh layer list
		lock = true;
		{
			int n = pDoc->image.GetLayerCount();
			globals.layers.DeleteAllItems();
			for (int i = 0; i < n; i++) {
				FGXLayer *layer = pDoc->image.GetLayer(i);

				int icon;
				switch (layer->GetType()) {
				case LayerNormal:
					icon = 0;
					break;

				case LayerAdjust:
					icon = 1;
					break;

				case LayerText:
					icon = 2;
					break;
				}

				globals.layers.InsertItem(0, layer->GetName(), icon);
				globals.layers.SetCheck(0, layer->IsVisible());
			}
			globals.layers.SetItemState(pDoc->image.GetLayerCount() - selLayer - 1, LVIS_SELECTED, LVIS_SELECTED);
			globals.layers.EnsureVisible(pDoc->image.GetLayerCount() - selLayer - 1, false);

			globals.channel.SetCurSel(pDoc->selChannel);
		}
		lock = false;
		break;

	case 1:
		// Toggle layer visibility
		{
			int i = pDoc->image.GetLayerCount() - lParam - 1;

			bool check = globals.layers.GetCheck(lParam);
			if (pDoc->image.GetLayer(i)->IsVisible() != check) {
				pDoc->image.GetLayer(i)->SetVisible(check);
				pDoc->RedrawLayer(i);
			}

			if (globals.layers.GetItemState(lParam, LVIS_SELECTED) == LVIS_SELECTED)
				SelectLayer(i);
		}
		break;

	case 2:
		// Display layer popup menu
		{
			CMenu menu;
			menu.LoadMenu(IDR_POPUP);

			CMenu *popup = menu.GetSubMenu(0);
			TranslateMenu(popup);
			popup->SetDefaultItem(ID_LAYER_PROPERTIES);
			if (pDoc->image.GetLayerCount() == 1) popup->EnableMenuItem(ID_LAYER_DELETE, MF_GRAYED);
			if (selLayer == pDoc->image.GetLayerCount() - 1) popup->EnableMenuItem(ID_LAYER_MOVEUP, MF_GRAYED);
			if (layer->GetType() != LayerText) popup->EnableMenuItem(ID_LAYER_RASTERIZE, MF_GRAYED);
			if (selLayer == 0) popup->EnableMenuItem(ID_LAYER_MOVEDOWN, MF_GRAYED);

			CPoint pt;
			::GetCursorPos(&pt);

			popup->TrackPopupMenu(TPM_LEFTALIGN, pt.x, pt.y, this);
		}
		break;

	case 3:
		// Open layer properties
		EditLayerProperties();
		break;

	case 4:
		// Change channel mask
		if (pDoc->selChannel != globals.channel.GetCurSel()) {
			pDoc->selChannel = globals.channel.GetCurSel();
			pDoc->channelMask = channelMasks[pDoc->selChannel];
			pDoc->Redraw();
		}
		break;

	case 5:
		// Reorder layers
		{
			NMDRAGINFO *pDI = (NMDRAGINFO *)lParam;

			int n = globals.layers.GetItemCount(),
				from = n - pDI->fromItem - 1,
				to = n - pDI->toItem - 1;

			pDoc->SaveUndoOrderLayer(from, to);
			pDoc->image.MoveLayer(from, to);
			SelectLayer(to);
			pDoc->RedrawLayer(selLayer);
			pDoc->SetModifiedFlag(true);
			PostMessage(WM_APP, 0, 0);
		}
		break;

	case 6:
		// Execute script
		FGXScript::Execute((LPCTSTR)lParam, *this);
		break;
	}

	return 0;
}

void CFotografixView::OnLayerNew()
{
	FGXLayer *layer = new FGXLayer;
	SelectLayer(pDoc->image.AddLayer(layer));

	CString name;
	name.Format(TEXT("%s %d"), LangItem(Layer), selLayer + 1);
	layer->SetName(name);

	pDoc->AddUndoNewLayer(selLayer);

	OnApp(0, 0);
	pDoc->SetModifiedFlag(true);
}

void CFotografixView::OnNewLayerviacopy()
{
	if (pDoc->selection.GetPosition().IsRectEmpty() == false) {
		FGXLayer *layer = new FGXLayer(pDoc->selection.GetPosition());
		this->layer->CopySelection(*layer, pDoc->selection);

		int i = pDoc->image.AddLayer(layer);
		pDoc->RedrawLayer(i);
		pDoc->AddUndoNewLayer(i);

		OnApp(0, 0);
		pDoc->SetModifiedFlag(true);
	}
}

void CFotografixView::OnUpdateNewLayerviacopy(CCmdUI *pCmdUI)
{
	pCmdUI->Enable(pDoc->selection.GetPosition().IsRectEmpty() == false);
}

void CFotografixView::OnNewLayerviacut()
{
	if (pDoc->selection.GetPosition().IsRectEmpty() == false) {
		if (layer->GetType() == LayerText)
			MessageBox(LangMessage(ErrorRasterize), NULL, MB_ICONWARNING | MB_OK);
		else {
			OnNewLayerviacopy();
			OnEditClear();
		}
	}
}

void CFotografixView::OnLayerDuplicatelayer()
{
	FGXLayer *layer = new FGXLayer(pDoc->selection.GetPosition());
	this->layer->Clone(*layer);
	layer->SetName(LangItemParam(CopySuffix, layer->GetName()));

	SelectLayer(pDoc->image.AddLayer(layer));
	pDoc->RedrawLayer(selLayer);
	pDoc->AddUndoNewLayer(selLayer, LangItem(DuplicateLayer));
	
	OnApp(0, 0);
	pDoc->SetModifiedFlag(true);
}

void CFotografixView::OnUpdateAdjustments(CCmdUI *pCmdUI)
{
	pCmdUI->Enable(layer->GetType() != LayerAdjust && pDoc->channelMask == ChannelAll);
}

void CFotografixView::OnLayerProperties()
{
	EditLayerProperties();
}

bool CFotografixView::EditLayerProperties(bool canUndo) {
	switch (layer->GetType()) {
	case LayerNormal:
		{
			CLayerDialog dlg;

			dlg.opacity = layer->GetOpacity() * 100 / 255;
			dlg.mode = layer->GetMode();

			if (dlg.DoModal() == IDOK) {
				if (canUndo) pDoc->SaveUndoModifyLayer(selLayer);
				layer->SetOpacity(dlg.opacity * 255 / 100);
				layer->SetMode(dlg.mode);
				pDoc->RedrawLayer(selLayer);
				pDoc->SetModifiedFlag(true);
				return true;
			}
		}
		break;

	case LayerAdjust:
		{
			CAdjustDialog dlg;
			if (InitAdjustDialog(dlg, layer->a)) {
				dlg.adjustLayer = true;

				dlg.def[0] = layer->a1;
				dlg.def[1] = layer->a2;
				dlg.def[2] = layer->a3;

				if (dlg.DoModal() == IDOK) {
					if (canUndo) pDoc->SaveUndoModifyLayer(selLayer);

					layer->a1 = dlg.value[0];
					layer->a2 = dlg.value[1];
					layer->a3 = dlg.value[2];

					pDoc->RedrawLayer(selLayer);
					pDoc->SetModifiedFlag(true);
					return true;
				}
			} else
				MessageBox(LangMessage(ErrorNoProps), NULL, MB_ICONINFORMATION | MB_OK);
		}
		break;

	case LayerText:
		{
			CTextDialog dlg;

			dlg.opacity = layer->GetOpacity() * 100 / 255;
			dlg.mode = layer->GetMode();

			dlg.logFont.lfHeight = -MulDiv(layer->a1, GetDeviceCaps(::GetDC(NULL), LOGPIXELSY), 72);
			_tcscpy(dlg.logFont.lfFaceName, layer->a4);
			if (layer->a3 & FontStyle::FontStyleBold) dlg.logFont.lfWeight = FW_BOLD;
			if (layer->a3 & FontStyle::FontStyleItalic) dlg.logFont.lfItalic = true;
			if (layer->a3 & FontStyle::FontStyleUnderline) dlg.logFont.lfUnderline = true;
			if (layer->a3 & FontStyle::FontStyleStrikeout) dlg.logFont.lfStrikeOut = true;
			if (layer->a3 & 128) dlg.aa = true;

			dlg.color = layer->a2;
			dlg.text = layer->a5;

			if (dlg.DoModal() == IDOK) {
				if (canUndo) pDoc->SaveUndoModifyLayer(selLayer);
				
				layer->SetOpacity(dlg.opacity * 255 / 100);
				layer->SetMode(dlg.mode);

				layer->a1 = -MulDiv(dlg.logFont.lfHeight, 72, GetDeviceCaps(::GetDC(NULL), LOGPIXELSY));
				layer->a4 = dlg.logFont.lfFaceName;
				layer->a3 = 0;
				if (dlg.logFont.lfWeight >= FW_BOLD) layer->a3 |= FontStyle::FontStyleBold;
				if (dlg.logFont.lfItalic > 0) layer->a3 |= FontStyle::FontStyleItalic;
				if (dlg.logFont.lfUnderline > 0) layer->a3 |= FontStyle::FontStyleUnderline;
				if (dlg.logFont.lfStrikeOut > 0) layer->a3 |= FontStyle::FontStyleStrikeout;
				if (dlg.aa == true) layer->a3 |= 128;

				layer->a2 = dlg.color;
				layer->a5 = dlg.text;
				layer->RenderText();

				pDoc->Redraw();
				pDoc->SetModifiedFlag(true);
				return true;
			}
		}
		break;
	}

	return false;
}

void CFotografixView::OnNewAdjustmentLayer(UINT nID)
{
	FGXLayer *layer = new FGXLayer(pDoc->bounds, LayerAdjust);
	layer->a = AdjustBrightnessContrast + (nID - ID_LAYER_BRIGHTNESS);

	CAdjustDialog dlg;
	bool opt = InitAdjustDialog(dlg, layer->a);

	if (opt) {
		layer->a1 = dlg.def[0];
		layer->a2 = dlg.def[1];
		layer->a3 = dlg.def[2];
	} else {
		layer->a1 = globals.fgColor.GetColor();
		layer->a2 = globals.bgColor.GetColor();
	}

	int i = pDoc->image.AddLayer(layer);
	if (pDoc->selection.GetPosition().IsRectEmpty() == false)
		layer->SetMask(pDoc->selection, pDoc->selection.GetPosition());
	//pDoc->RedrawLayer(i);

	if (opt) {
		dlg.layer = i;
		dlg.adjustLayer = true;

		if (dlg.DoModal() == IDCANCEL) {
			pDoc->image.DeleteLayer(i);
			pDoc->Redraw();
			return;
		}

		layer->a1 = dlg.value[0];
		layer->a2 = dlg.value[1];
		layer->a3 = dlg.value[2];
	}

	pDoc->RedrawLayer(i);
	layer->SetName(dlg.title + TEXT(' ') + LangItem(Layer));
	pDoc->AddUndoNewLayer(i, LangItem(NewAdjLayer));

	OnApp(0, 0);
	pDoc->SetModifiedFlag(true);
}

void CFotografixView::OnLayerDelete()
{
	pDoc->AddUndoDeleteLayer(selLayer);

	pDoc->image.DeleteLayer(selLayer);
	SelectLayer(0);

	pDoc->Redraw();

	OnApp(0, 0);
	pDoc->SetModifiedFlag(true);
}

void CFotografixView::OnUpdateLayerDelete(CCmdUI *pCmdUI)
{
	pCmdUI->Enable(pDoc->image.GetLayerCount() > 1);
}

void CFotografixView::OnLayerRename()
{
	CInputDialog dlg;

	dlg.title = LangItem(RenameLayer);
	dlg.prompt = LangItem(Name);
	dlg.value = layer->GetName();

	if (dlg.DoModal() == IDOK) {
		layer->SetName(dlg.value);
		OnApp(0, 0);
		pDoc->SetModifiedFlag(true);
	}
}

void CFotografixView::OnUpdateStatusLayer(CCmdUI *pCmdUI)
{
	CString str;
	str.Format(TEXT("%s (%s)"), layer->GetName(), channelText[pDoc->selChannel]);
	pCmdUI->SetText(str);
}

void CFotografixView::OnUpdateStatusSize(CCmdUI *pCmdUI)
{
	pCmdUI->SetText(sizeText);
}

void CFotografixView::OnUpdateStatusZoom(CCmdUI *pCmdUI)
{
	pCmdUI->SetText(zoomText);
}

void CFotografixView::OnLayerAddlayermask()
{
	if (layer->GetType() != LayerAdjust && layer->HasMask() == false) {
		if (layer->GetType() == LayerText)
			MessageBox(LangMessage(ErrorRasterize), NULL, MB_ICONWARNING | MB_OK);
		else {
			pDoc->SaveUndoAddLayerMask(selLayer);
			layer->AddMask();
			if (pDoc->selection.GetPosition().IsRectEmpty() == false) {
				layer->SetMask(pDoc->selection, pDoc->selection.GetPosition());
				pDoc->RedrawLayer(selLayer);
			}
			pDoc->SetModifiedFlag(true);
		}
	}
}

void CFotografixView::OnUpdateLayerAddlayermask(CCmdUI *pCmdUI)
{
	pCmdUI->Enable(layer->GetType() != LayerAdjust && layer->HasMask() == false);
}

void CFotografixView::OnLayerRemovelayermask_Helper(bool apply)
{
	if (layer->HasMask() == true) {
		if (apply == true) {
			pDoc->SaveUndoLayer(selLayer, LangItem(DeleteMask));
			layer->DelMask(true);
			pDoc->SetModifiedFlag(true);
		} else {
			pDoc->SaveUndoLayer(selLayer, LangItem(DeleteMask));
			layer->DelMask(false);
			pDoc->RedrawLayer(selLayer);
			pDoc->SetModifiedFlag(true);
		}
	}
}

void CFotografixView::OnLayerRemovelayermask()
{
	switch (MessageBox(LangMessage(AskApplyMask), NULL, MB_YESNOCANCEL)) {
	case IDYES:
		OnLayerRemovelayermask_Helper(true);
		break;

	case IDNO:
		OnLayerRemovelayermask_Helper(false);
		break;
	}
}

void CFotografixView::OnUpdateLayerRemovelayermask(CCmdUI *pCmdUI)
{
	pCmdUI->Enable(layer->HasMask() == true);
}

void CFotografixView::OnLayerEnablelayermask()
{
	if (layer->HasMask() == true) {
		layer->EnableMask(!layer->IsMaskEnabled());
		pDoc->RedrawLayer(selLayer);
		pDoc->SetModifiedFlag(true);
	}
}

void CFotografixView::OnUpdateLayerEnablelayermask(CCmdUI *pCmdUI)
{
	pCmdUI->Enable(layer->HasMask() == true);
	pCmdUI->SetCheck(layer->IsMaskEnabled());
}

void CFotografixView::OnEditFill()
{
	if (layer->GetType() == LayerText) {
		MessageBox(LangMessage(ErrorRasterize), NULL, MB_ICONWARNING | MB_OK);
		return;
	}

	//layer->MagicFill(pDoc->GetUndoLayer(selLayer, TEXT("Magic Fill")), pDoc->selection, pDoc->channelMask);
	//pDoc->RedrawLayerSelection(selLayer);
	//return;

	CFillDialog dlg;
	if (dlg.DoModal() == IDOK) {
		FGXColor color;
		BYTE opacity = dlg.opacity * 255 / 100;

		switch (dlg.colour) {
		case 0:
			color = FGXColor(globals.fgColor.GetColor(), opacity);
			break;

		case 1:
			color = FGXColor(globals.bgColor.GetColor(), opacity);
			break;

		case 2:
			color = FGXColor(0, 0, 0, opacity);
			break;

		case 3:
			color = FGXColor(128, 128, 128, opacity);
			break;

		case 4:
			color = FGXColor(255, 255, 255, opacity);
			break;
		}

		pDoc->SaveUndoLayer(selLayer, LangItem(Fill));

		if (pDoc->selection.GetPosition().IsRectEmpty() == true) {
			layer->SetPosition(pDoc->bounds);
			layer->Fill(color, pDoc->channelMask);
			pDoc->RedrawLayer(selLayer);
		} else {
			layer->EnsureRect(pDoc->selection.GetPosition());
			layer->Clear(FGXLayer(), pDoc->selection, color, pDoc->channelMask);
			pDoc->RedrawLayerSelection(selLayer);
		}

		pDoc->SetModifiedFlag(true);
	}
}

void CFotografixView::OnImageImagesize()
{
	CSizeDialog dlg;

	dlg.ow = pDoc->image.GetWidth();
	dlg.oh = pDoc->image.GetHeight();
	dlg.res = pDoc->image.GetResolution();
	dlg.unit = pDoc->image.GetUnit();

	if (dlg.DoModal() == IDOK) {
		FGXImage &undo = pDoc->GetUndoImage(LangItem(ImageSize));

		if (dlg.resample == true)
			pDoc->image.ResizeImage(undo, dlg.w, dlg.h);
		else
			pDoc->image.Clone(undo);

		pDoc->image.SetResolution(dlg.res);
		pDoc->image.SetUnit(dlg.unit);

		pDoc->Redraw(true);
		pDoc->SetModifiedFlag(true);
	}
}

void CFotografixView::OnFileSave()
{
	if (pDoc->hasPath == false)
		OnFileSaveAs();
	else {
		if (pDoc->IsModified()) pDoc->dirty = true;
		pDoc->image.Compact();
		pDoc->DoSave(pDoc->GetPathName());
		pDoc->SetModifiedFlag(false);
	}
}

void CFotografixView::OnFileSaveAs()
{
	LPCTSTR filter = TEXT("\
Fotografix (*.fgx)|*.fgx|\
JPEG (*.jpg)|*.jpg|\
Bitmap (*.bmp)|*.bmp|\
PNG (*.png)|*.png|\
GIF (*.gif)|*.gif|\
TIFF (*.tif)|*.tif|\
Targa (*.tga)|*.tga|\
RAW (*.raw)|*.raw||\
");

	LPCTSTR ext[] = {
		TEXT(".fgx"),
		TEXT(".jpg"),
		TEXT(".bmp"),
		TEXT(".png"),
		TEXT(".gif"),
		TEXT(".tif"),
		TEXT(".tga"),
		TEXT(".raw")
	};

	CFileDialog dlg(false, NULL, NULL, OFN_OVERWRITEPROMPT, filter, this);

#ifndef _DEBUG
	// Remove MFC hook to enable new Vista dialog
	dlg.m_ofn.Flags &= ~OFN_ENABLEHOOK;
#endif

	if (dlg.DoModal() == IDOK) {
		CString path = dlg.GetPathName();

		LPCTSTR exp = ext[dlg.m_ofn.nFilterIndex - 1];
		if (path.Right(4).MakeLower() != exp) path += exp;

		// RAW save doesn't count
		if (dlg.m_ofn.nFilterIndex == 8)
			pDoc->OnSaveDocument(path);
		else {
			if (pDoc->IsModified())
				pDoc->dirty = true;

			pDoc->image.Compact();

			if (pDoc->DoSave(path) == true) {
				pDoc->SetPathName(path);
				pDoc->hasPath = true;
				pDoc->SetModifiedFlag(false);
			}
		}
	}
}

void CFotografixView::OnFlipHorizontal()
{
	pDoc->SaveUndoFlipImage(0, LangItem(FlipH));
	pDoc->image.Flip(true, false);
	pDoc->Redraw();
	pDoc->SetModifiedFlag(true);
}

void CFotografixView::OnFlipVertical()
{
	pDoc->SaveUndoFlipImage(1, LangItem(FlipV));
	pDoc->image.Flip(false, true);
	pDoc->Redraw();
	pDoc->SetModifiedFlag(true);
}

void CFotografixView::OnLayerMoveup()
{
	if (selLayer < pDoc->image.GetLayerCount() - 1) {
		pDoc->SaveUndoOrderLayer(selLayer, selLayer + 1);
		pDoc->image.MoveLayer(selLayer, selLayer + 1);
		pDoc->RedrawLayer(++selLayer);
		OnApp(0, 0);
		pDoc->SetModifiedFlag(true);
	}
}

void CFotografixView::OnLayerMovedown()
{
	if (selLayer > 0) {
		pDoc->SaveUndoOrderLayer(selLayer, selLayer - 1);
		pDoc->image.MoveLayer(selLayer, selLayer - 1);
		pDoc->RedrawLayer(--selLayer);
		OnApp(0, 0);
		pDoc->SetModifiedFlag(true);
	}
}

void CFotografixView::OnSelectLayertransparency()
{
	pDoc->CleanSelection();
	pDoc->selection.LoadLayer(pDoc->GetUndoSelection(), *layer);
	pDoc->PrepareSelection();
}

void CFotografixView::OnLayerRasterize()
{
	pDoc->SaveUndoRasterize(selLayer);
	layer->Rasterize();
	OnApp(0, 0);
}

void CFotografixView::OnUpdateLayerRasterize(CCmdUI *pCmdUI)
{
	pCmdUI->Enable(layer->GetType() == LayerText);
}

void CFotografixView::OnModify_Helper(int distance, LPCTSTR undoText) {
	if (pDoc->selection.GetPosition().IsRectEmpty() == false) {
		pDoc->CleanSelection();

		CRect rect = pDoc->selection.GetPosition();
		rect.InflateRect(distance, distance);

		pDoc->selection.ResizeTo(pDoc->GetUndoSelection(undoText), rect);
		pDoc->PrepareSelection();
	}
}

void CFotografixView::OnModifyExpand()
{
	CInputDialog dlg;

	dlg.title = LangItem(ExpandSel);
	dlg.prompt = LangItem(Distance);
	dlg.value = TEXT("1");
	dlg.number = true;

	if (dlg.DoModal() == IDOK)
		OnModify_Helper(_ttoi(dlg.value), LangItem(ExpandSel));
}

void CFotografixView::OnModifyContract()
{
	CInputDialog dlg;

	dlg.title = LangItem(ContractSel);
	dlg.prompt = LangItem(Distance);
	dlg.value = TEXT("1");
	dlg.number = true;

	if (dlg.DoModal() == IDOK)
		OnModify_Helper(-_ttoi(dlg.value), LangItem(ContractSel));
}

void CFotografixView::OnFeather_Helper(int radius) {
	if (pDoc->selection.GetPosition().IsRectEmpty() == false) {
		pDoc->CleanSelection();

		CRect rect = pDoc->selection.GetPosition();
		rect.InflateRect(radius*2, radius*2);

		pDoc->selection.ExpandTo(pDoc->GetUndoSelection(LangItem(FeatherSel)), rect);
		pDoc->selection.Adjust(FGXChannel(), pDoc->selection.GetPosition(), FilterBlur, radius, 0, 0);

		pDoc->PrepareSelection();
	}
}

void CFotografixView::OnModifyFeather()
{
	CInputDialog dlg;

	dlg.title = LangItem(FeatherSel);
	dlg.prompt = LangItem(Radius);
	dlg.value = TEXT("5");
	dlg.number = true;

	if (dlg.DoModal() == IDOK)
		OnFeather_Helper(_ttoi(dlg.value));
}

void CFotografixView::OnEditFillFg()
{
	if (layer->GetType() == LayerText) {
		MessageBox(LangMessage(ErrorRasterize), NULL, MB_ICONWARNING | MB_OK);
		return;
	}

	pDoc->SaveUndoLayer(selLayer, LangItem(Fill));

	if (pDoc->selection.GetPosition().IsRectEmpty() == true) {
		layer->SetPosition(pDoc->bounds);
		layer->Fill(FGXColor(globals.fgColor.GetColor(), 255), pDoc->channelMask);
		pDoc->RedrawLayer(selLayer);
	} else {
		layer->EnsureRect(pDoc->selection.GetPosition());
		layer->Clear(FGXLayer(), pDoc->selection, FGXColor(globals.fgColor.GetColor(), 255), pDoc->channelMask);
		pDoc->RedrawLayerSelection(selLayer);
	}

	pDoc->SetModifiedFlag(true);
}

void CFotografixView::OnEditFillBg()
{
	if (layer->GetType() == LayerText) {
		MessageBox(LangMessage(ErrorRasterize), NULL, MB_ICONWARNING | MB_OK);
		return;
	}

	pDoc->SaveUndoLayer(selLayer, LangItem(Fill));

	if (pDoc->selection.GetPosition().IsRectEmpty() == true) {
		layer->SetPosition(pDoc->bounds);
		layer->Fill(FGXColor(globals.bgColor.GetColor(), 255), pDoc->channelMask);
		pDoc->RedrawLayer(selLayer);
	} else {
		layer->EnsureRect(pDoc->selection.GetPosition());
		layer->Clear(FGXLayer(), pDoc->selection, FGXColor(globals.bgColor.GetColor(), 255), pDoc->channelMask);
		pDoc->RedrawLayerSelection(selLayer);
	}

	pDoc->SetModifiedFlag(true);
}

void CFotografixView::OnRotate90()
{
	pDoc->image.Rotate(pDoc->GetUndoImage(LangItem(Rotate)), 90);
	pDoc->Redraw(true);
	pDoc->SetModifiedFlag(true);
}

void CFotografixView::OnRotate180()
{
	pDoc->image.Rotate(pDoc->GetUndoImage(LangItem(Rotate)), 180);
	pDoc->Redraw(true);
	pDoc->SetModifiedFlag(true);
}

void CFotografixView::OnRotate270()
{
	pDoc->image.Rotate(pDoc->GetUndoImage(LangItem(Rotate)), 270);
	pDoc->Redraw(true);
	pDoc->SetModifiedFlag(true);
}

void CFotografixView::OnRotateOther()
{
	CInputDialog dlg;

	dlg.title = LangItem(Rotate);
	dlg.prompt = LangItem(Angle);
	dlg.value = TEXT("45");

	if (dlg.DoModal() == IDOK) {
		pDoc->image.Rotate(pDoc->GetUndoImage(LangItem(Rotate)), _ttoi(dlg.value));
		pDoc->Redraw(true);
		pDoc->SetModifiedFlag(true);
	}
}

void CFotografixView::OnLayerMergedown()
{
	if (selLayer > 0) {
		FGXLayer *layer2 = pDoc->image.GetLayer(selLayer - 1);
		layer2->EnsureRect(layer->GetPosition());

		pDoc->SaveUndoLayer(selLayer - 1, LangItem(MergeDown));
		layer->AlphaRender(*layer2, layer->GetPosition());

		pDoc->AddUndoDeleteLayer(selLayer);
		pDoc->image.DeleteLayer(selLayer);

		SelectLayer(selLayer - 1);

		OnApp(0, 0);
		pDoc->SetModifiedFlag(true);
	}
}

void CFotografixView::OnUpdateLayerMergedown(CCmdUI *pCmdUI)
{
	pCmdUI->Enable(selLayer > 0);
}

void CFotografixView::OnLayerFlattenimage()
{
	pDoc->image.Flatten(pDoc->GetUndoImage(LangItem(FlattenImage)));
	SelectLayer(0);

	pDoc->Redraw();
	OnApp(0, 0);
	pDoc->SetModifiedFlag(true);
}

void CFotografixView::OnUpdateLayerFlattenimage(CCmdUI *pCmdUI)
{
	pCmdUI->Enable(pDoc->image.GetLayerCount() > 1);
}

BOOL CFotografixView::OnMouseWheel(UINT nFlags, short zDelta, CPoint point)
{
	if (dragging == false) {
		TransformPoint(point);
		if (zDelta < 0)
			OnViewZoomout();
		else
			OnViewZoomin();
		if (dx == 0 && dy == 0)
			ScrollToPosition(CPoint(point.x * zoom - cx / 2, point.y * zoom - cy / 2));
	}

	return true;
}
