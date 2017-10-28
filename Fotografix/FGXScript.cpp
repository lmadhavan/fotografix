#include "StdAfx.h"
#include "FotografixView.h"
#include "FGXScript.h"
#include "Language.h"

static LPCTSTR commands[] = {
	TEXT("FillFG"),				// 0
	TEXT("FillBG"),
	TEXT("Cut"),
	TEXT("Copy"),
	TEXT("Paste"),
	TEXT("Clear"),				// 5
	TEXT("Crop"),
	TEXT("RevealAll"),
	TEXT("FlipH"),
	TEXT("FlipV"),
	TEXT("NewLayer"),			// 10
	TEXT("NewLayerCopy"),
	TEXT("NewLayerCut"),
	TEXT("DuplicateLayer"),
	TEXT("DeleteLayer"),
	TEXT("AddLayerMask"),		// 15
	TEXT("RemoveLayerMask:Apply"),
	TEXT("RemoveLayerMask:Discard"),
	TEXT("MoveLayerUp"),
	TEXT("MoveLayerDown"),
	TEXT("RasterizeLayer"),		// 20
	TEXT("SelectAll"),
	TEXT("Deselect"),
	TEXT("SelectInverse"),
	TEXT("SelectTransparency"),
	TEXT("SelectExpand"),		// 25
	TEXT("SelectContract"),
	TEXT("SelectFeather"),
	TEXT("ShowLayer"),
	TEXT("HideLayer"),
	TEXT("SetLayerOpacity"),	// 30
	TEXT("SelectRectangle"),
	TEXT("SelectRectangle:Add"),
	TEXT("SelectRectangle:Subtract"),
	TEXT("SelectEllipse"),
	TEXT("SelectEllipse:Add"),	// 35
	TEXT("SelectEllipse:Subtract")
};

static LPCTSTR adjustments[] = {
	TEXT("BrightnessContrast"),	// 0
	TEXT("ColourBalance"),
	TEXT("Desaturate"),
	TEXT("BlackWhite"),
	TEXT("GradientMap"),
	TEXT("Invert"),				// 5
	TEXT("Threshold"),
	TEXT("Posterize"),
	TEXT("Levels"),
	TEXT("Blur"),
	TEXT("MotionBlur"),			// 10
	TEXT("AddNoise"),
	TEXT("Emboss"),
	TEXT("Diffuse"),
	TEXT("Solarize"),
	TEXT("Offset"),				// 15
	TEXT("Pixelate"),
	TEXT("NightVision"),
	TEXT("Halftone"),
	TEXT("Sharpen"),
	TEXT("UnsharpMask"),		// 20
	TEXT("EdgesAll"),
	TEXT("EdgesHorz"),
	TEXT("EdgesVert"),
	TEXT("EdgesDiag")
};

static const int numCommands = 37;
static const int numAdjustments = 25;

CMapStringToPtr FGXScript::map;

void FGXScript::Initialize() {
	for (int i = 0; i < numCommands; i++)
		map[commands[i]] = reinterpret_cast<void *>(i);

	for (int i = 0; i < numAdjustments; i++)
		map[adjustments[i]] = reinterpret_cast<void *>(1000 + i);
}

int FGXScript::GetValue(CString str, CFotografixView &target) {
	if (str.GetLength() == 0) return 0;

	wchar_t *ptr;
	float i = wcstod(str, &ptr);

	switch (str[str.GetLength() - 1]) {
	case 'w': return i * target.layer->GetPosition().Width();
	case 'h': return i * target.layer->GetPosition().Height();
	case 'W': return i * target.pDoc->image.GetWidth();
	case 'H': return i * target.pDoc->image.GetHeight();
	default: return i;
	}
}

#define TokenString() str.Tokenize(TEXT(" \t"), pos)
#define TokenInt() GetValue(TokenString(), target)

bool FGXScript::Execute(LPCTSTR scriptPath, CFotografixView &target) {
	CStdioFile file;

	if (file.Open(scriptPath, CFile::modeRead | CFile::shareDenyWrite)) {
		CString str;
		while (file.ReadString(str)) {
			str.Trim();
			if (str.IsEmpty() || *(LPCTSTR)str == ';') continue;

			int pos = 0;
			CString cmd = str.Tokenize(TEXT(" \t"), pos);

			void *v;
			if (map.Lookup(cmd, v)) {
				int i = reinterpret_cast<int>(v);

				if (i > 1000) {
					if (target.layer->GetType() == LayerText) {
						target.MessageBox(LangMessage(ErrorRasterize), NULL, MB_ICONWARNING | MB_OK);
						continue;
					}

					CAdjustDialog dlg;

					if (target.InitAdjustDialog(dlg, AdjustBrightnessContrast + (i - 1000))) {
						if (dlg.num == 0) {
							dlg.value[0] = globals.fgColor.GetColor();
							dlg.value[1] = globals.bgColor.GetColor();
						} else {
											 dlg.value[0] = TokenInt();
							if (dlg.num > 1) dlg.value[1] = TokenInt();
							if (dlg.num > 2) dlg.value[2] = TokenInt();
						}
					}

					if (target.pDoc->selection.GetPosition().IsRectEmpty() == true) {
						target.layer->Adjust(target.pDoc->GetUndoLayer(target.selLayer, dlg.title), dlg.adjustType, dlg.value[0], dlg.value[1], dlg.value[2], target.pDoc->channelMask);
						target.pDoc->RedrawLayer(target.selLayer);
					} else {
						target.layer->Adjust(target.pDoc->GetUndoLayer(target.selLayer, dlg.title), target.pDoc->selection, dlg.adjustType, dlg.value[0], dlg.value[1], dlg.value[2], target.pDoc->channelMask);
						target.pDoc->RedrawLayerSelection(target.selLayer);
					}

					target.pDoc->SetModifiedFlag(true);
					continue;
				}

				switch (i) {
				case 0:
					target.OnEditFillFg();
					break;

				case 1:
					target.OnEditFillBg();
					break;

				case 2:
					target.OnEditCut();
					break;

				case 3:
					target.OnEditCopy();
					break;

				case 4:
					target.OnEditPaste();
					break;

				case 5:
					target.OnEditClear();
					break;

				case 6:
					target.OnImageCrop();
					break;

				case 7:
					target.OnImageRevealall();
					break;

				case 8:
					target.OnFlipHorizontal();
					break;

				case 9:
					target.OnFlipVertical();
					break;

				case 10:
					target.OnLayerNew();
					break;

				case 11:
					target.OnNewLayerviacopy();
					break;

				case 12:
					target.OnNewLayerviacut();
					break;

				case 13:
					target.OnLayerDuplicatelayer();
					break;

				case 14:
					target.OnLayerDelete();
					break;

				case 15:
					target.OnLayerAddlayermask();
					break;

				case 16:
					target.OnLayerRemovelayermask_Helper(true);
					break;

				case 17:
					target.OnLayerRemovelayermask_Helper(false);
					break;

				case 18:
					target.OnLayerMoveup();
					break;

				case 19:
					target.OnLayerMovedown();
					break;

				case 20:
					target.OnLayerRasterize();
					break;

				case 21:
					target.OnSelectAll();
					break;

				case 22:
					target.OnSelectDeselect();
					break;

				case 23:
					target.OnSelectInverse();
					break;

				case 24:
					target.OnSelectLayertransparency();
					break;

				case 25:
					target.OnModify_Helper(TokenInt(), LangItem(ExpandSel));
					break;

				case 26:
					target.OnModify_Helper(-TokenInt(), LangItem(ContractSel));
					break;

				case 27:
					target.OnFeather_Helper(TokenInt());
					break;

				case 28:
					target.layer->SetVisible(true);
					target.pDoc->RedrawLayer(target.selLayer);
					target.OnApp(0, 0);
					break;

				case 29:
					target.layer->SetVisible(false);
					target.pDoc->RedrawLayer(target.selLayer);
					target.OnApp(0, 0);
					break;

				case 30:
					target.layer->SetOpacity(TokenInt() * 255 / 100);
					target.pDoc->RedrawLayer(target.selLayer);
					break;

				case 31:
				case 32:
				case 33:
					{
						int l = TokenInt();
						int t = TokenInt();
						int r = TokenInt();
						int b = TokenInt();

						target.pDoc->CleanSelection();
						target.pDoc->selection.SelectRectangle(target.pDoc->GetUndoSelection(LangItem(RSelection)), CRect(l, t, r, b), SelectType(i - 31));
						target.pDoc->PrepareSelection();
					}
					break;

				case 34:
				case 35:
				case 36:
					{
						int l = TokenInt();
						int t = TokenInt();
						int r = TokenInt();
						int b = TokenInt();

						target.pDoc->CleanSelection();
						target.pDoc->selection.SelectEllipse(target.pDoc->GetUndoSelection(LangItem(ESelection)), CRect(l, t, r, b), SelectType(i - 34));
						target.pDoc->PrepareSelection();
					}
					break;
				}
			} else {
				if (target.MessageBox(LangMessageParam(ErrorScript, cmd), NULL, MB_YESNO | MB_ICONWARNING) == IDNO)
					return false;
			}
		}

		return true;
	}

	return false;
}

#undef TokenInt
#undef TokenString
