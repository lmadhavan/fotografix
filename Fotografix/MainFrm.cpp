// MainFrm.cpp : implementation of the CMainFrame class
//

#include "stdafx.h"
#include "Fotografix.h"

#include "MainFrm.h"
#include "FotografixDoc.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif

const int firstTool = ID_TOOL_RSELECT,
		  lastTool = ID_TOOL_ZOOM;

HCURSOR toolCursor[16];

LPCTSTR toolBtnName[9] = {
	TEXT("New"),
	TEXT("Open"),
	TEXT("Save"),
	NULL,
	TEXT("Cut"),
	TEXT("Copy"),
	TEXT("Paste"),
	NULL,
	TEXT("Print")
};

LPCTSTR toolName[15] = {
	TEXT("RSelect"),
	TEXT("ESelect"),
	TEXT("MagicWand"),
	TEXT("Move"),
	TEXT("Transform"),
	TEXT("Ruler"),
	TEXT("Brush"),
	TEXT("Eraser"),
	TEXT("Clone"),
	TEXT("Fill"),
	TEXT("Gradient"),
	TEXT("Text"),
	TEXT("Dropper"),
	TEXT("Pan"),
	TEXT("Zoom")
};

TCHAR toolKey[15] = {
	'S', 'E', 'W',
	'V', 'R', 'L',
	'B', 'Z', 'C',
	'F', 'G', 'T',
	'Y', 'H', 'J'
};

CString toolMessage[15];

// CMainFrame

IMPLEMENT_DYNAMIC(CMainFrame, CMDIFrameWnd)

BEGIN_MESSAGE_MAP(CMainFrame, CMDIFrameWnd)
	ON_WM_CREATE()
	ON_WM_DESTROY()

	// Global help commands
	//ON_COMMAND(ID_HELP_FINDER, &CMDIFrameWnd::OnHelpFinder)
	//ON_COMMAND(ID_HELP, &CMDIFrameWnd::OnHelp)
	//ON_COMMAND(ID_CONTEXT_HELP, &CMDIFrameWnd::OnContextHelp)
	//ON_COMMAND(ID_DEFAULT_HELP, &CMDIFrameWnd::OnHelpFinder)

	ON_COMMAND(ID_VIEW_FULLSCREEN, &CMainFrame::OnViewFullscreen)
	ON_UPDATE_COMMAND_UI(ID_VIEW_FULLSCREEN, &CMainFrame::OnUpdateViewFullscreen)

	ON_COMMAND_RANGE(ID_TOOL_RSELECT, lastTool, OnSelectTool)
	ON_UPDATE_COMMAND_UI_RANGE(ID_TOOL_RSELECT, lastTool, OnUpdateTool)

	ON_COMMAND(IDC_DEFAULT, OnDefaultColors)
	ON_COMMAND(IDC_SWAP, OnSwapColors)
	ON_COMMAND(IDC_FGCOLOR, OnChangeColor)

	ON_CBN_SELCHANGE(IDC_SELSTYLE, OnSelchangeSelstyle)
	ON_EN_CHANGE(IDC_SELW, OnChangeSelD)
	ON_EN_CHANGE(IDC_SELH, OnChangeSelD)
	ON_UPDATE_COMMAND_UI(IDC_SELW, OnUpdateSelD)
	ON_UPDATE_COMMAND_UI(IDC_SELH, OnUpdateSelD)

	ON_EN_CHANGE(IDC_TOLERANCE, OnChangeTolerance)
	ON_COMMAND(IDC_WAND, OnContiguous)

	ON_CBN_SELCHANGE(IDC_BRUSH, OnSelchangeBrush)
	ON_EN_CHANGE(IDC_OPACITY, OnChangeBrushOpacity)
	ON_CBN_SELCHANGE(IDC_MODE, OnSelchangeBrushMode)

	ON_CBN_SELCHANGE(IDC_GRADCOLOR, OnSelchangeGradcolor)
	ON_CBN_SELCHANGE(IDC_GRADTYPE, OnSelchangeGradtype)

	ON_NOTIFY(LVN_ITEMCHANGED, IDC_LAYERS, OnLayerNotify)
	ON_NOTIFY(NM_RCLICK, IDC_LAYERS, OnLayerNotify)
	ON_NOTIFY(NM_DBLCLK, IDC_LAYERS, OnLayerNotify)
	ON_NOTIFY(LVNEX_FINISHDRAG, IDC_LAYERS, OnLayerNotify)
	ON_CBN_SELCHANGE(IDC_CHANNEL, OnSelchangeChannel)

	ON_NOTIFY(NM_RCLICK, IDC_SCRIPTS, OnScriptNotify)
	ON_NOTIFY(NM_DBLCLK, IDC_SCRIPTS, OnScriptNotify)

	ON_COMMAND(ID_WINDOW_COLOUR, &CMainFrame::OnWindowColour)
	ON_UPDATE_COMMAND_UI(ID_WINDOW_COLOUR, &CMainFrame::OnUpdateWindowColour)
	ON_COMMAND(ID_WINDOW_LAYERS, &CMainFrame::OnWindowLayers)
	ON_UPDATE_COMMAND_UI(ID_WINDOW_LAYERS, &CMainFrame::OnUpdateWindowLayers)
	ON_COMMAND(ID_WINDOW_TOOLS, &CMainFrame::OnWindowTools)
	ON_UPDATE_COMMAND_UI(ID_WINDOW_TOOLS, &CMainFrame::OnUpdateWindowTools)
	ON_COMMAND(ID_WINDOW_SCRIPTS, &CMainFrame::OnWindowScripts)
	ON_UPDATE_COMMAND_UI(ID_WINDOW_SCRIPTS, &CMainFrame::OnUpdateWindowScripts)

	ON_UPDATE_COMMAND_UI(ID_INDICATOR_LAYER, &CMainFrame::OnUpdateStatusDummy)
	ON_UPDATE_COMMAND_UI(ID_INDICATOR_CHANNEL, &CMainFrame::OnUpdateStatusDummy)
	ON_UPDATE_COMMAND_UI(ID_INDICATOR_SIZE, &CMainFrame::OnUpdateStatusDummy)
	ON_UPDATE_COMMAND_UI(ID_INDICATOR_ZOOM, &CMainFrame::OnUpdateStatusDummy)

	ON_MESSAGE(WM_APP, &CMainFrame::OnIdle)
	ON_COMMAND(IDC_NEXTBRUSH, &CMainFrame::OnNextbrush)
	ON_COMMAND(IDC_PREVBRUSH, &CMainFrame::OnPrevbrush)
	ON_COMMAND(ID_HELP_FINDER, &CMainFrame::OnHelp)
	ON_NOTIFY_EX_RANGE(TTN_NEEDTEXT, 0, 0xFFFF, &CMainFrame::OnToolTipText)
END_MESSAGE_MAP()

static UINT indicators[] =
{
	ID_SEPARATOR,           // status line indicator
	ID_INDICATOR_LAYER,
	ID_INDICATOR_SIZE,
	ID_INDICATOR_ZOOM
	//ID_INDICATOR_CAPS,
	//ID_INDICATOR_NUM,
	//ID_INDICATOR_SCRL
};


// CMainFrame construction/destruction

CMainFrame::CMainFrame()
{
	CWinApp *app = AfxGetApp();

	toolCursor[0] = toolCursor[1]
				  = toolCursor[2]
				  = app->LoadCursor(IDC_CROSS2);

	toolCursor[3] = app->LoadCursor(IDC_MOVE);
	toolCursor[4] = app->LoadStandardCursor(MAKEINTRESOURCE(IDC_NO));

	toolCursor[5] = toolCursor[6]
				  = toolCursor[7]
				  = toolCursor[8]
				  = toolCursor[9]
				  = toolCursor[10]
				  = toolCursor[0];

	toolCursor[11] = app->LoadCursor(IDC_TEXT);
	toolCursor[12] = app->LoadCursor(IDC_DROPPER);
	toolCursor[13] = app->LoadCursor(IDC_HAND2);
	toolCursor[14] = app->LoadCursor(IDC_ZOOM);
	toolCursor[15] = app->LoadCursor(IDC_HAND3);

	globals.selStyle = app->GetProfileInt(TEXT("Marquee"), TEXT("Type"), 0);
	globals.selW = globals.selH = 1;

	globals.wandTolerance = app->GetProfileInt(TEXT("Wand"), TEXT("Tolerance"), 32);
	globals.wandContiguous = app->GetProfileInt(TEXT("Wand"), TEXT("Contiguous"), 1);

	globals.gradColor = app->GetProfileInt(TEXT("Gradient"), TEXT("Transparent"), 0);
	globals.gradType = app->GetProfileInt(TEXT("Gradient"), TEXT("Type"), 0);

	globals.textSize = app->GetProfileInt(TEXT("Text"), TEXT("Size"), 12);
	globals.textColor = app->GetProfileInt(TEXT("Text"), TEXT("Colour"), 0);
	globals.textStyle = app->GetProfileInt(TEXT("Text"), TEXT("Style"), 128);
	globals.textFace = app->GetProfileString(TEXT("Text"), TEXT("Face"), TEXT("Times New Roman"));

	globals.imageList.Create(16, 16, ILC_COLOR32 | ILC_MASK, 5, 1);
	globals.imageList.Add(app->LoadIcon(IDI_LAYER));
	globals.imageList.Add(app->LoadIcon(IDI_ADJUST));
	globals.imageList.Add(app->LoadIcon(IDI_TEXT));
	globals.imageList.Add(app->LoadIcon(IDI_FOLDER));
	globals.imageList.Add(app->LoadIcon(IDI_FILE));

	m_pBar = NULL;
}

CMainFrame::~CMainFrame()
{
}

void CMainFrame::DockControlBarEx(CControlBar* pBar, UINT nDockBarID, CControlBar *pPrev)
{
	RecalcLayout(true);

	CRect rect;
	pPrev->GetWindowRect(rect);

	if (nDockBarID == AFX_IDW_DOCKBAR_LEFT || nDockBarID == AFX_IDW_DOCKBAR_RIGHT)
		rect.OffsetRect(0, 1);
	else
		rect.OffsetRect(1, 0);

	DockControlBar(pBar, nDockBarID, rect);
}

int CMainFrame::OnCreate(LPCREATESTRUCT lpCreateStruct)
{
	if (CMDIFrameWnd::OnCreate(lpCreateStruct) == -1)
		return -1;

	CWinApp *app = AfxGetApp();

	for (int i = 0; i < 15; i++) {
		CString helpName = CString(toolName[i]) + TEXT("_help");
		toolMessage[i].Format(TEXT("%s: %s"), GetLangItem(toolName[i]), GetLangItem(helpName));
	}

	if (!m_wndToolBar.CreateEx(this, TBSTYLE_LIST, WS_CHILD | WS_VISIBLE | /*CBRS_GRIPPER | */CBRS_TOOLTIPS | CBRS_FLYBY | CBRS_ALIGN_TOP) ||
		!m_wndToolBar.LoadToolBar(IDR_MAINFRAME))
	{
		TRACE0("Failed to create toolbar\n");
		return -1;      // fail to create
	}
	for (int i = 0; i < 9; i++) {
		UINT id, style;
		int image;
		m_wndToolBar.GetButtonInfo(i, id, style, image);
		m_wndToolBar.SetButtonStyle(i, style | BTNS_AUTOSIZE);

		if (toolBtnName[i] != NULL)
			m_wndToolBar.SetButtonText(i, GetMenuItem(toolBtnName[i]));
	}

	// Tools bar

	if (!m_wndToolsBar.CreateEx(this, TBSTYLE_FLAT, WS_CHILD | WS_VISIBLE | CBRS_GRIPPER | CBRS_SIZE_FIXED | CBRS_TOOLTIPS | CBRS_FLYBY | CBRS_ALIGN_LEFT) ||
		!m_wndToolsBar.LoadToolBar(IDR_TOOLS))
	{
		TRACE0("Failed to create toolbar\n");
		return -1;      // fail to create
	}

	m_wndToolsBar.SetWindowText(TEXT("Tools"));
	TranslateDialog(&m_wndToolsBar);

	{
		UINT style = m_wndToolsBar.GetButtonStyle(0) | TBBS_WRAPPED;
		m_wndToolsBar.SetButtonStyle(2, style);
		m_wndToolsBar.SetButtonStyle(5, style);
		m_wndToolsBar.SetButtonStyle(8, style);
		m_wndToolsBar.SetButtonStyle(11, style);
	}

	// Color window

	if (!m_wndColor.Create(this, IDR_COLOR, 
		CBRS_GRIPPER | CBRS_ALIGN_LEFT, AFX_IDW_DIALOGBAR))
	{
		TRACE0("Failed to create dialogbar\n");
		return -1;		// fail to create
	}

	m_wndColor.SetWindowText(TEXT("Colour"));
	TranslateDialog(&m_wndColor);

	globals.fgColor.SubclassDlgItem(IDC_FGCOLOR, &m_wndColor);
	globals.fgColor.SetColor(app->GetProfileInt(TEXT("Colours"), TEXT("Foreground"), RGB(0, 0, 0)));

	globals.bgColor.SubclassDlgItem(IDC_BGCOLOR, &m_wndColor);
	globals.bgColor.SetColor(app->GetProfileInt(TEXT("Colours"), TEXT("Background"), RGB(255, 255, 255)));

	// Selection window

	if (!m_wndSelection.Create(this, IDR_SELECT, 
		CBRS_GRIPPER | CBRS_ALIGN_TOP, AFX_IDW_DIALOGBAR + 1))
	{
		TRACE0("Failed to create dialogbar\n");
		return -1;		// fail to create
	}

	m_wndSelection.SetWindowText(TEXT("Selection"));
	TranslateDialog(&m_wndSelection);
	m_selStyle.Attach(m_wndSelection.GetDlgItem(IDC_SELSTYLE)->m_hWnd);
	m_selStyle.SetCurSel(globals.selStyle);

	if (!m_wndWand.Create(this, IDR_WAND, 
		CBRS_GRIPPER | CBRS_ALIGN_TOP, AFX_IDW_DIALOGBAR + 2))
	{
		TRACE0("Failed to create dialogbar\n");
		return -1;		// fail to create
	}

	m_wndWand.SetWindowText(TEXT("MagicWand"));
	TranslateDialog(&m_wndWand);
	{
		CString str;
		str.Format(TEXT("%d"), globals.wandTolerance);
		m_wndWand.SetDlgItemText(IDC_TOLERANCE, str);
	}
	m_wndWand.CheckDlgButton(IDC_WAND, globals.wandContiguous);

	if (!m_wndBrush.Create(this, IDR_BRUSH, 
		CBRS_GRIPPER | CBRS_ALIGN_TOP, AFX_IDW_DIALOGBAR + 3))
	{
		TRACE0("Failed to create dialogbar\n");
		return -1;		// fail to create
	}

	m_wndBrush.SetWindowText(TEXT("Brush"));
	TranslateDialog(&m_wndBrush);
	brushPath = globals.appPath + TEXT("\\Brushes");
	m_brush.Attach(m_wndBrush.GetDlgItem(IDC_BRUSH)->m_hWnd);
	LoadBrushes(m_brush, brushPath);
	m_brush.SetCurSel(app->GetProfileInt(TEXT("Brush"), TEXT("Brush"), 0));
	{
		CString str;
		str.Format(TEXT("%d"), app->GetProfileInt(TEXT("Brush"), TEXT("Opacity"), 100));
		m_wndBrush.SetDlgItemText(IDC_OPACITY, str);
	}
	m_brushMode.Attach(m_wndBrush.GetDlgItem(IDC_MODE)->m_hWnd);
	m_brushMode.SetCurSel(app->GetProfileInt(TEXT("Brush"), TEXT("Mode"), 0));
	OnSelchangeBrush();

	if (!m_wndGradient.Create(this, IDR_GRADIENT, 
		CBRS_GRIPPER | CBRS_ALIGN_TOP, AFX_IDW_DIALOGBAR + 3))
	{
		TRACE0("Failed to create dialogbar\n");
		return -1;		// fail to create
	}

	m_wndGradient.SetWindowText(TEXT("Gradient"));
	TranslateDialog(&m_wndGradient);
	m_gradColor.Attach(m_wndGradient.GetDlgItem(IDC_GRADCOLOR)->m_hWnd);
	m_gradColor.SetCurSel(globals.gradColor);
	m_gradType.Attach(m_wndGradient.GetDlgItem(IDC_GRADTYPE)->m_hWnd);
	m_gradType.SetCurSel(globals.gradType);

	if (!m_wndDummy.Create(this, IDR_DUMMY, 
		CBRS_GRIPPER | CBRS_ALIGN_TOP, AFX_IDW_DIALOGBAR + 4))
	{
		TRACE0("Failed to create dialogbar\n");
		return -1;		// fail to create
	}

	if (!m_wndLayers.Create(this, IDR_LAYERS, 
		CBRS_GRIPPER | CBRS_ALIGN_RIGHT, AFX_IDW_DIALOGBAR + 5))
	{
		TRACE0("Failed to create dialogbar\n");
		return -1;		// fail to create
	}

	m_wndLayers.SetWindowText(TEXT("Layers"));
	TranslateDialog(&m_wndLayers);
	m_header[0].SubclassDlgItem(IDC_HEADER1, &m_wndLayers);
	m_header[1].SubclassDlgItem(IDC_HEADER2, &m_wndLayers);
	globals.layers.SubclassDlgItem(IDC_LAYERS, &m_wndLayers);
	globals.layers.SetExtendedStyle(LVS_EX_FULLROWSELECT | LVS_EX_CHECKBOXES);
	globals.layers.SetImageList(&globals.imageList, LVSIL_SMALL);
	globals.layers.InsertColumn(0, TEXT("Name"), LVCFMT_LEFT, 180, 0);
	globals.channel.Attach(m_wndLayers.GetDlgItem(IDC_CHANNEL)->m_hWnd);
	for (int i = 0; i < 8; i++) {
		channelText[i] = GetLangItem(channelNames[i]);
		globals.channel.AddString(channelText[i]);
	}

	if (!m_wndScripts.Create(this, IDR_SCRIPTS, 
		CBRS_GRIPPER | CBRS_ALIGN_RIGHT, AFX_IDW_DIALOGBAR + 6))
	{
		TRACE0("Failed to create dialogbar\n");
		return -1;		// fail to create
	}

	m_wndScripts.SetWindowText(TEXT("Scripts"));
	TranslateDialog(&m_wndScripts);
	m_header[2].SubclassDlgItem(IDC_HEADER1, &m_wndScripts);
	globals.scripts.Attach(m_wndScripts.GetDlgItem(IDC_SCRIPTS)->m_hWnd);
	globals.scripts.SetImageList(&globals.imageList, TVSIL_NORMAL);
	LoadScripts(globals.scripts, TVI_ROOT, globals.appPath + TEXT("\\Scripts"));

	m_font.CreatePointFont(110, TEXT("Tahoma"));
	if (!m_wndStatusBar.Create(this) ||
		(m_wndStatusBar.SetFont(&m_font), !m_wndStatusBar.SetIndicators(indicators,
		  sizeof(indicators)/sizeof(UINT))))
	{
		TRACE0("Failed to create status bar\n");
		return -1;      // fail to create
	}

	CStatusBarCtrl &bar = m_wndStatusBar.GetStatusBarCtrl();
	bar.SetMinHeight(36);
	bar.SetIcon(0, (HICON)::LoadImage(app->m_hInstance, MAKEINTRESOURCE(IDR_MAINFRAME), IMAGE_ICON, 32, 32, LR_SHARED));
	bar.SetIcon(1, (HICON)::LoadImage(app->m_hInstance, MAKEINTRESOURCE(IDI_LAYER), IMAGE_ICON, 16, 16, LR_SHARED));
	bar.SetIcon(2, (HICON)::LoadImage(app->m_hInstance, MAKEINTRESOURCE(IDI_SIZE), IMAGE_ICON, 16, 16, LR_SHARED));
	bar.SetIcon(3, (HICON)::LoadImage(app->m_hInstance, MAKEINTRESOURCE(IDI_ZOOM), IMAGE_ICON, 16, 16, LR_SHARED));

	m_wndToolBar.SetBarStyle(m_wndToolBar.GetBarStyle() |
		CBRS_TOOLTIPS | CBRS_FLYBY);

	EnableDocking(CBRS_ALIGN_ANY);

	//m_wndToolBar.EnableDocking(CBRS_ALIGN_TOP);
	m_wndToolsBar.EnableDocking(CBRS_ALIGN_LEFT);
	m_wndColor.EnableDocking(CBRS_ALIGN_LEFT);
	m_wndSelection.EnableDocking(CBRS_ALIGN_TOP);
	m_wndWand.EnableDocking(CBRS_ALIGN_TOP);
	m_wndBrush.EnableDocking(CBRS_ALIGN_TOP);
	m_wndGradient.EnableDocking(CBRS_ALIGN_TOP);
	m_wndDummy.EnableDocking(CBRS_ALIGN_TOP);
	m_wndLayers.EnableDocking(CBRS_ALIGN_RIGHT);
	m_wndScripts.EnableDocking(CBRS_ALIGN_RIGHT);

	//DockControlBar(&m_wndToolBar, AFX_IDW_DOCKBAR_TOP);
	DockControlBar(&m_wndToolsBar, AFX_IDW_DOCKBAR_LEFT);
	DockControlBarEx(&m_wndColor, AFX_IDW_DOCKBAR_LEFT, &m_wndToolsBar);
	DockControlBar(&m_wndLayers, AFX_IDW_DOCKBAR_RIGHT);
	DockControlBarEx(&m_wndScripts, AFX_IDW_DOCKBAR_RIGHT, &m_wndLayers);
	DockControlBar(&m_wndSelection, AFX_IDW_DOCKBAR_TOP);
	DockControlBar(&m_wndWand, AFX_IDW_DOCKBAR_TOP);
	DockControlBar(&m_wndBrush, AFX_IDW_DOCKBAR_TOP);
	DockControlBar(&m_wndGradient, AFX_IDW_DOCKBAR_TOP);
	DockControlBar(&m_wndDummy, AFX_IDW_DOCKBAR_TOP);

	//CDockState state;
	//state.LoadState(TEXT("Toolbars"));
	//SetDockState(state);

	//FloatControlBar(&m_wndLayers, CPoint(::GetSystemMetrics(SM_CXSCREEN) - 260, 120));

	//m_wndToolBar.ShowWindow(SW_HIDE);
	m_wndSelection.ShowWindow(SW_HIDE);
	m_wndWand.ShowWindow(SW_HIDE);
	m_wndBrush.ShowWindow(SW_HIDE);
	m_wndGradient.ShowWindow(SW_HIDE);
	m_wndDummy.ShowWindow(SW_HIDE);

	globals.curTool = app->GetProfileInt(TEXT("Tools"), TEXT("Tool"), firstTool);
	if (globals.curTool < firstTool || globals.curTool > lastTool) globals.curTool = firstTool;
	OnSelectTool(globals.curTool);

	return 0;
}

void CMainFrame::OnDestroy()
{
	CWinApp *app = AfxGetApp();

	app->WriteProfileInt(TEXT("Tools"), TEXT("Tool"), globals.curTool);
	app->WriteProfileInt(TEXT("Marquee"), TEXT("Type"), globals.selStyle);
	app->WriteProfileInt(TEXT("Brush"), TEXT("Brush"), m_wndBrush.GetDlgItem(IDC_BRUSH)->SendMessage(CB_GETCURSEL));
	app->WriteProfileInt(TEXT("Brush"), TEXT("Opacity"), m_wndBrush.GetDlgItemInt(IDC_OPACITY, NULL, false));
	app->WriteProfileInt(TEXT("Brush"), TEXT("Mode"), m_brushMode.GetCurSel());
	app->WriteProfileInt(TEXT("Wand"), TEXT("Tolerance"), globals.wandTolerance);
	app->WriteProfileInt(TEXT("Wand"), TEXT("Contiguous"), globals.wandContiguous);
	app->WriteProfileInt(TEXT("Gradient"), TEXT("Transparent"), globals.gradColor);
	app->WriteProfileInt(TEXT("Gradient"), TEXT("Type"), globals.gradType);

	app->WriteProfileInt(TEXT("Colours"), TEXT("Foreground"), globals.fgColor.GetColor());
	app->WriteProfileInt(TEXT("Colours"), TEXT("Background"), globals.bgColor.GetColor());

	app->WriteProfileInt(TEXT("Text"), TEXT("Size"), globals.textSize);
	app->WriteProfileInt(TEXT("Text"), TEXT("Colour"), globals.textColor);
	app->WriteProfileInt(TEXT("Text"), TEXT("Style"), globals.textStyle);
	app->WriteProfileString(TEXT("Text"), TEXT("Face"), globals.textFace);

	bool max = IsZoomed();
	app->WriteProfileInt(TEXT("Window"), TEXT("Maximized"), max);
	if (max == false) {
		CRect rect;
		GetWindowRect(rect);
		app->WriteProfileInt(TEXT("Window"), TEXT("Left"), rect.left);
		app->WriteProfileInt(TEXT("Window"), TEXT("Top"), rect.top);
		app->WriteProfileInt(TEXT("Window"), TEXT("Width"), rect.Width());
		app->WriteProfileInt(TEXT("Window"), TEXT("Height"), rect.Height());
	}

	//CDockState state;
	//GetDockState(state);
	//state.SaveState(TEXT("Toolbars"));

	m_gradType.Detach();
	m_gradColor.Detach();
	m_brushMode.Detach();
	m_brush.Detach();
	m_selStyle.Detach();
	globals.layers.Detach();
	globals.channel.Detach();
	globals.scripts.Detach();

	CMDIFrameWnd::OnDestroy();
}

BOOL CMainFrame::PreCreateWindow(CREATESTRUCT& cs)
{
	if( !CMDIFrameWnd::PreCreateWindow(cs) )
		return FALSE;

	cs.style = WS_OVERLAPPED | WS_CAPTION | FWS_ADDTOTITLE
		 | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX | WS_MAXIMIZE | WS_SYSMENU;

	return TRUE;
}


// CMainFrame diagnostics

#ifdef _DEBUG
void CMainFrame::AssertValid() const
{
	CMDIFrameWnd::AssertValid();
}

void CMainFrame::Dump(CDumpContext& dc) const
{
	CMDIFrameWnd::Dump(dc);
}

#endif //_DEBUG


// CMainFrame message handlers


void CMainFrame::OnViewFullscreen()
{
	ShowWindow(SW_SHOWMINIMIZED);

	if (GetStyle() & WS_POPUP)
		ModifyStyle(WS_POPUP, WS_CAPTION | WS_THICKFRAME | WS_SYSMENU);
	else
		ModifyStyle(WS_CAPTION | WS_THICKFRAME | WS_SYSMENU, WS_POPUP);

	ShowWindow(SW_SHOWMAXIMIZED);
}

void CMainFrame::OnUpdateViewFullscreen(CCmdUI *pCmdUI)
{
	pCmdUI->SetCheck(GetStyle() & WS_POPUP);
}

void CMainFrame::OnDefaultColors() {
	globals.fgColor.SetColor(RGB(0, 0, 0));
	globals.bgColor.SetColor(RGB(255, 255, 255));
	OnChangeColor();
}

void CMainFrame::OnSwapColors() {
	COLORREF fg = globals.fgColor.GetColor();
	globals.fgColor.SetColor(globals.bgColor.GetColor());
	globals.bgColor.SetColor(fg);
	OnChangeColor();
}

void CMainFrame::OnChangeColor() {
	globals.brush.Fill(FGXColor(globals.fgColor.GetColor(), 255));
}

void CMainFrame::OnSelchangeSelstyle() {
	globals.selStyle = m_selStyle.GetCurSel();
}

void CMainFrame::OnChangeSelD() {
	globals.selW = max(1, m_wndSelection.GetDlgItemInt(IDC_SELW, NULL, false));
	globals.selH = max(1, m_wndSelection.GetDlgItemInt(IDC_SELH, NULL, false));
}

void CMainFrame::OnUpdateSelD(CCmdUI *pCmdUI)
{
	pCmdUI->Enable(globals.selStyle != 0);
}

void CMainFrame::OnChangeTolerance() {
	globals.wandTolerance = m_wndWand.GetDlgItemInt(IDC_TOLERANCE, NULL, false);
}

void CMainFrame::OnContiguous() {
	globals.wandContiguous = m_wndWand.IsDlgButtonChecked(IDC_WAND) == BST_CHECKED;
}

void CMainFrame::OnSelchangeBrush() {
	CString name;
	m_wndBrush.GetDlgItemText(IDC_BRUSH, name);

	CString path;
	path.Format(TEXT("%s\\%s.png"), brushPath, name);

	Bitmap bitmap(path);
	if (bitmap.GetLastStatus() == Status::Ok) {
		CFotografixDoc::LoadImage_Bitmap(bitmap, globals.brush);
		globals.brush.ConvertToBrush();
		OnChangeColor();
	}
}

void CMainFrame::OnChangeBrushOpacity() {
	int opacity = m_wndBrush.GetDlgItemInt(IDC_OPACITY, NULL, false);
	if (opacity < 0) opacity = 0; else if (opacity > 100) opacity = 100;
	globals.brush.SetOpacity(opacity * 255 / 100);
}

void CMainFrame::OnSelchangeBrushMode() {
	globals.brush.SetMode(m_brushMode.GetCurSel());
}

void CMainFrame::OnSelchangeGradcolor() {
	globals.gradColor = m_gradColor.GetCurSel();
}

void CMainFrame::OnSelchangeGradtype() {
	globals.gradType = m_gradType.GetCurSel();
}

void CMainFrame::OnSelectTool(UINT nID)
{
	globals.curTool = nID;
	globals.curCursor = toolCursor[globals.curTool - firstTool];

	if (m_pBar != NULL) {
		ShowControlBar(m_pBar, false, true);
		m_pBar = NULL;
	}

	switch (nID) {
	case ID_TOOL_RSELECT:
	case ID_TOOL_ESELECT:
		m_pBar = &m_wndSelection;
		break;

	case ID_TOOL_WAND:
	case ID_TOOL_FILL:
		m_pBar = &m_wndWand;
		break;

	case ID_TOOL_BRUSH:
	case ID_TOOL_ERASER:
	case ID_TOOL_CLONE:
		m_pBar = &m_wndBrush;
		break;

	case ID_TOOL_GRADIENT:
		m_pBar = &m_wndGradient;
		break;

	default:
		m_pBar = &m_wndDummy;
		break;
	}

	if (m_pBar != NULL)
		ShowControlBar(m_pBar, true, true);
}

void CMainFrame::OnUpdateTool(CCmdUI *pCmdUI)
{
	pCmdUI->SetCheck(globals.curTool == pCmdUI->m_nID);
}

void CMainFrame::OnWindowColour()
{
	ShowControlBar(&m_wndColor, !m_wndColor.IsWindowVisible(), false);
}

void CMainFrame::OnUpdateWindowColour(CCmdUI *pCmdUI)
{
	pCmdUI->SetCheck(m_wndColor.IsWindowVisible());
}

void CMainFrame::OnWindowLayers()
{
	ShowControlBar(&m_wndLayers, !m_wndLayers.IsWindowVisible(), false);
}

void CMainFrame::OnUpdateWindowLayers(CCmdUI *pCmdUI)
{
	pCmdUI->SetCheck(m_wndLayers.IsWindowVisible());
}

void CMainFrame::OnWindowTools()
{
	ShowControlBar(&m_wndToolsBar, !m_wndToolsBar.IsWindowVisible(), false);
}

void CMainFrame::OnUpdateWindowTools(CCmdUI *pCmdUI)
{
	pCmdUI->SetCheck(m_wndToolsBar.IsWindowVisible());
}

void CMainFrame::OnWindowScripts()
{
	ShowControlBar(&m_wndScripts, !m_wndScripts.IsWindowVisible(), false);
}

void CMainFrame::OnUpdateWindowScripts(CCmdUI *pCmdUI)
{
	pCmdUI->SetCheck(m_wndScripts.IsWindowVisible());
}

void CMainFrame::OnLayerNotify(NMHDR *pNMHDR, LRESULT *pResult) {
	static int dragItem = -1;

	switch (pNMHDR->code) {
	case LVN_ITEMCHANGED:
		{
			NMLISTVIEW *pLV = (NMLISTVIEW *)pNMHDR;
			if (pLV->iItem > -1 && (pLV->uChanged & LVIF_STATE))
				MDIGetActive()->GetWindow(GW_CHILD)->PostMessage(WM_APP, 1, pLV->iItem);
		}
		break;

	case NM_RCLICK:
		{
			int i = globals.layers.GetNextItem(-1, LVNI_SELECTED);
			if (i > -1)
				MDIGetActive()->GetWindow(GW_CHILD)->PostMessage(WM_APP, 2, 0);
		}
		break;

	case NM_DBLCLK:
		{
			int i = globals.layers.GetNextItem(-1, LVNI_SELECTED);
			if (i > -1) {
				CPoint pt;
				::GetCursorPos(&pt);
				globals.layers.ScreenToClient(&pt);

				UINT flags;
				globals.layers.HitTest(pt, &flags);

				if ((flags & LVHT_ONITEMLABEL) > 0)
					MDIGetActive()->GetWindow(GW_CHILD)->PostMessage(WM_APP, 3, 0);
			}
		}
		break;

	case LVNEX_FINISHDRAG:
		MDIGetActive()->GetWindow(GW_CHILD)->SendMessage(WM_APP, 5, (LPARAM)pNMHDR);
		break;
	}
}

void CMainFrame::OnScriptNotify(NMHDR *pNMHDR, LRESULT *pResult) {
	switch (pNMHDR->code) {
	//case NM_RCLICK:
	//	{
	//		int i = globals.scripts.GetNextItem(-1, LVNI_SELECTED);
	//		if (i > -1) {
	//			MDIGetActive()->GetWindow(GW_CHILD)->PostMessage(WM_APP + 1, 0, 0);
	//		}
	//	}
	//	break;

	case NM_DBLCLK:
		if (MDIGetActive() != NULL) {
			HTREEITEM item = globals.scripts.GetSelectedItem();

			if (globals.scripts.GetItemData(item) == 0) {
				CString path;
				while (item != NULL) {
					path = globals.scripts.GetItemText(item) + '\\' + path;
					item = globals.scripts.GetParentItem(item);
				}
				path.TrimRight('\\');
				path = globals.appPath + TEXT("\\Scripts\\") + path + TEXT(".fgs");
			
				MDIGetActive()->GetWindow(GW_CHILD)->PostMessage(WM_APP, 6, (LPARAM)(LPCTSTR)path);
			}
		}
		break;
	}
}

void CMainFrame::OnSelchangeChannel() {
	if (MDIGetActive() != NULL) MDIGetActive()->GetWindow(GW_CHILD)->PostMessage(WM_APP, 4, 0);
}

void CMainFrame::LoadBrushes(CWnd &target, const CString &path) {
	CFileFind find;

	bool more = find.FindFile(path + TEXT("\\*.png"));
	while (more) {
		more = find.FindNextFile();
		target.SendMessage(CB_ADDSTRING, 0, (LPARAM)(LPCTSTR)find.GetFileTitle());
	}
}

void CMainFrame::LoadScripts(CTreeCtrl &target, HTREEITEM root, const CString &path) {
	CFileFind find;

	bool more = find.FindFile(path + TEXT("\\*.*"));
	while (more) {
		more = find.FindNextFile();

		if (find.IsDots())
			continue;
		
		if (find.IsDirectory())
			LoadScripts(target, target.InsertItem(TVIF_TEXT | TVIF_IMAGE | TVIF_SELECTEDIMAGE | TVIF_PARAM, find.GetFileName(), 3, 3, 0, 0, 1, root, TVI_SORT), find.GetFilePath());
		else if (find.GetFileName().Right(4).CompareNoCase(TEXT(".fgs")) == 0)
			target.InsertItem(TVIF_TEXT | TVIF_IMAGE | TVIF_SELECTEDIMAGE | TVIF_PARAM, find.GetFileTitle(), 4, 4, 0, 0, 0, root, TVI_SORT);
	}
}

void CMainFrame::OnUpdateStatusDummy(CCmdUI *pCmdUI)
{
	pCmdUI->SetText(TEXT(""));
}

LRESULT CMainFrame::OnIdle(WPARAM wParam, LPARAM lParam) {
	if (m_nIDTracking == 0 || m_nIDTracking == AFX_IDS_IDLEMESSAGE)
		SetMessageText(toolMessage[globals.curTool - firstTool]);

	return 0;
}

void CMainFrame::OnNextbrush()
{
	if (m_brush.GetCurSel() < m_brush.GetCount() - 1) {
		m_brush.SetCurSel(m_brush.GetCurSel() + 1);
		OnSelchangeBrush();
	}
}

void CMainFrame::OnPrevbrush()
{
	if (m_brush.GetCurSel() > 0) {
		m_brush.SetCurSel(m_brush.GetCurSel() - 1);
		OnSelchangeBrush();
	}
}

void CMainFrame::OnHelp()
{
	::ShellExecute(NULL, NULL, TEXT("http://lmadhavan.com/software/fotografix/help/?ref=fotografix141"), NULL, NULL, SW_SHOWNORMAL);
}

/* from MFC source (winfrm.cpp) - modified to support strings outside string table */
BOOL CMainFrame::OnToolTipText(UINT, NMHDR* pNMHDR, LRESULT* pResult)
{
	ENSURE_ARG(pNMHDR != NULL);
	ENSURE_ARG(pResult != NULL);
	ASSERT(pNMHDR->code == TTN_NEEDTEXTW);

	TOOLTIPTEXTW* pTTTW = (TOOLTIPTEXTW*)pNMHDR;
	CString strTipText;
	UINT_PTR nID = pNMHDR->idFrom;
	if (pNMHDR->code == TTN_NEEDTEXTW && (pTTTW->uFlags & TTF_IDISHWND))
	{
		// idFrom is actually the HWND of the tool
		nID = (UINT)(WORD)::GetDlgCtrlID((HWND)nID);
	}

	if (nID >= firstTool && nID <= lastTool) // will be zero on a separator
	{
		nID -= firstTool;
		strTipText.Format(TEXT("%s (%c)"), GetLangItem(toolName[nID]), toolKey[nID]);
	}

	Checked::wcsncpy_s(pTTTW->szText, _countof(pTTTW->szText), strTipText, _TRUNCATE);
	*pResult = 0;

	// bring the tooltip window above other popup windows
	::SetWindowPos(pNMHDR->hwndFrom, HWND_TOP, 0, 0, 0, 0,
		SWP_NOACTIVATE|SWP_NOSIZE|SWP_NOMOVE|SWP_NOOWNERZORDER);

	return TRUE;    // message was handled
}
