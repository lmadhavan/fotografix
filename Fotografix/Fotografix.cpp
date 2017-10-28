// Fotografix.cpp : Defines the class behaviors for the application.
//

#include "stdafx.h"
#include "Fotografix.h"
#include "MainFrm.h"

#include "ChildFrm.h"
#include "FotografixDoc.h"
#include "FotografixView.h"
#include "FGXScript.h"

#include "Language.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif

// CFotografixApp

BEGIN_MESSAGE_MAP(CFotografixApp, CWinApp)
	ON_COMMAND(ID_APP_ABOUT, &CFotografixApp::OnAppAbout)
	// Standard file based document commands
	ON_COMMAND(ID_FILE_NEW, &CWinApp::OnFileNew)
	ON_COMMAND(ID_FILE_OPEN, &CFotografixApp::OnFileOpen)
	ON_COMMAND(ID_FILE_EXTRACT, &CFotografixApp::OnFileExtract)
	// Standard print setup command
	ON_COMMAND(ID_FILE_PRINT_SETUP, &CWinApp::OnFilePrintSetup)
	ON_COMMAND(ID_APP_LANGUAGE, &CFotografixApp::OnAppLanguage)
END_MESSAGE_MAP()


// CFotografixApp construction

CFotografixApp::CFotografixApp()
{
	//EnableHtmlHelp();

	// TODO: add construction code here,
	// Place all significant initialization in InitInstance
}


// The one and only CFotografixApp object

CFotografixApp theApp;

Globals globals;


// CFotografixApp initialization

BOOL CFotografixApp::InitInstance()
{
	// InitCommonControlsEx() is required on Windows XP if an application
	// manifest specifies use of ComCtl32.dll version 6 or later to enable
	// visual styles.  Otherwise, any window creation will fail.
	INITCOMMONCONTROLSEX InitCtrls;
	InitCtrls.dwSize = sizeof(InitCtrls);
	// Set this to include all the common control classes you want to use
	// in your application.
	InitCtrls.dwICC = ICC_WIN95_CLASSES;
	InitCommonControlsEx(&InitCtrls);

	CWinApp::InitInstance();

	::GdiplusStartup(&gdiToken, &GdiplusStartupInput(), NULL);

	::GetModuleFileName(NULL, globals.appPath.GetBuffer(MAX_PATH), MAX_PATH);
	globals.appPath.ReleaseBuffer();
	globals.appPath = globals.appPath.Left(globals.appPath.ReverseFind('\\'));
	globals.openExtract = false;

	FGXScript::Initialize();

	// Standard initialization
	// If you are not using these features and wish to reduce the size
	// of your final executable, you should remove from the following
	// the specific initialization routines you do not need
	// Change the registry key under which our settings are stored
	// TODO: You should modify this string to be something appropriate
	// such as the name of your company or organization
	free((void *)m_pszProfileName);
	m_pszProfileName = _tcsdup(globals.appPath + TEXT("\\Fotografix.ini"));
	LoadStdProfileSettings(4);  // Load standard INI file options (including MRU)

	CString langFile = GetProfileString(TEXT("Window"), TEXT("LanguageFile"), TEXT("lang_en.ini"));
	if (langFile.Find('\\') == -1) langFile = globals.appPath + '\\' + langFile;
	if (LoadLanguage(langFile) == false)
		if (LoadLanguage(globals.appPath + '\\' + TEXT("lang_en.ini")) == false)
			MessageBox(NULL, TEXT("Unable to find any language files. Most UI text will be missing. It is strongly recommended that you obtain a language file from the Fotografix website."), NULL, MB_ICONWARNING | MB_OK);

	// Register the application's document templates.  Document templates
	//  serve as the connection between documents, frame windows and views
	CMultiDocTemplate* pDocTemplate;
	pDocTemplate = new CMultiDocTemplate(IDR_DOCTYPE,
		RUNTIME_CLASS(CFotografixDoc),
		RUNTIME_CLASS(CChildFrame), // custom MDI child frame
		RUNTIME_CLASS(CFotografixView));
	if (!pDocTemplate)
		return FALSE;
	TranslateMenu(CMenu::FromHandle(pDocTemplate->m_hMenuShared));
	AddDocTemplate(pDocTemplate);

	// create main MDI Frame window
	CMainFrame* pMainFrame = new CMainFrame;
	if (!pMainFrame || !pMainFrame->LoadFrame(IDR_MAINFRAME))
	{
		delete pMainFrame;
		return FALSE;
	}
	TranslateMenu(CMenu::FromHandle(pMainFrame->m_hMenuDefault));
	m_pMainWnd = pMainFrame;

	// call DragAcceptFiles only if there's a suffix
	//  In an MDI app, this should occur immediately after setting m_pMainWnd
	// Enable drag/drop open
	m_pMainWnd->DragAcceptFiles();

	// Enable DDE Execute open
	//EnableShellOpen();
	//RegisterShellFileTypes(TRUE);

	// Parse command line for standard shell commands, DDE, file open
	CCommandLineInfo cmdInfo;
	ParseCommandLine(cmdInfo);


	// Dispatch commands specified on the command line.  Will return FALSE if
	// app was launched with /RegServer, /Register, /Unregserver or /Unregister.
	if (!ProcessShellCommand(cmdInfo))
		return FALSE;

	// The main window has been initialized, so show and update it
	bool max = GetProfileInt(TEXT("Window"), TEXT("Maximized"), 1);
	if (max == true)
		pMainFrame->ShowWindow(SW_SHOWMAXIMIZED);
	else {
		pMainFrame->ShowWindow(SW_RESTORE);
		pMainFrame->MoveWindow(
			GetProfileInt(TEXT("Window"), TEXT("Left"), 0),
			GetProfileInt(TEXT("Window"), TEXT("Top"), 0),
			GetProfileInt(TEXT("Window"), TEXT("Width"), 800),
			GetProfileInt(TEXT("Window"), TEXT("Height"), 600)
		);
	}
	pMainFrame->UpdateWindow();

	return TRUE;
}

int CFotografixApp::ExitInstance()
{
	::GdiplusShutdown(gdiToken);

	return CWinApp::ExitInstance();
}


// CAboutDlg dialog used for App About

class CAboutDlg : public CDialog
{
public:
	CAboutDlg();
	afx_msg void OnFeedback(NMHDR *pNMHDR, LRESULT *pResult);

// Dialog Data
	enum { IDD = IDD_ABOUTBOX };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support

// Implementation
protected:
	DECLARE_MESSAGE_MAP()
public:
	afx_msg void OnLButtonUp(UINT nFlags, CPoint point);
public:
	CString extra;
};

CAboutDlg::CAboutDlg() : CDialog(CAboutDlg::IDD)
{
	LangTranslate(TEXT("LangInfo"), extra);
}

void CAboutDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	DDX_Text(pDX, IDC_EXTRA, extra);
}

void CAboutDlg::OnLButtonUp(UINT nFlags, CPoint point)
{
	EndDialog(IDOK);
}

BEGIN_MESSAGE_MAP(CAboutDlg, CDialog)
	ON_NOTIFY(NM_CLICK, IDC_FEEDBACK, &CAboutDlg::OnFeedback)
	ON_WM_LBUTTONUP()
END_MESSAGE_MAP()

void CAboutDlg::OnFeedback(NMHDR *pNMHDR, LRESULT *pResult) {
	::ShellExecute(NULL, NULL, TEXT("http://lmadhavan.com/contact/?ref=fotografix141"), NULL, NULL, SW_SHOWNORMAL);
}

// App command to run the dialog
void CFotografixApp::OnAppAbout()
{
	CAboutDlg aboutDlg;
	aboutDlg.DoModal();
}


// CFotografixApp message handlers

void CFotografixApp::OnFileOpen()
{
	LPCTSTR filter = TEXT("\
Fotografix (*.fgx)|*.fgx|\
JPEG (*.jpg;*.jpeg;*.jpe;*.jfif)|*.jpg;*.jpeg;*.jpe;*.jfif|\
Bitmap (*.bmp)|*.bmp|\
PNG (*.png)|*.png|\
GIF (*.gif)|*.gif|\
TIFF (*.tif;*.tiff)|*.tif;*.tiff|\
Photoshop (*.psd)|*.psd|\
GIMP (*.xcf)|*.xcf|\
Targa (*.tga;*.vda;*.icb;*.vst)|*.tga;*.vda;*.icb;*.vst|\
PCX (*.pcx)|*.pcx|\
Icons (*.ico;*.cur)|*.ico;*.cur|\
RAW (*.raw)|*.raw|\
All Supported Formats|*.fgx;*.psd;*.xcf;*.jpg;*.jpeg;*.jpe;*.jfif;*.bmp;*.png;*.gif;*.tif;*.tiff;*.tga;*.pcx;*.ico;*.cur;*.raw|\
All Files (*.*)|*.*||\
");

	CFileDialog dlg(true, TEXT(".fgx"), NULL, OFN_ALLOWMULTISELECT | OFN_FILEMUSTEXIST, filter, GetMainWnd());

#ifndef _DEBUG
	// Remove MFC hook to enable new Vista dialog
	dlg.m_ofn.Flags &= ~OFN_ENABLEHOOK;
#endif

	dlg.m_ofn.nFilterIndex = 13;

	TCHAR buffer[4096];
	buffer[0] = 0;
	dlg.m_ofn.lpstrFile = buffer;
	dlg.m_ofn.nMaxFile = 4096;

	if (dlg.DoModal() == IDOK) {
		bool readOnly = dlg.GetReadOnlyPref();

		POSITION pos = dlg.GetStartPosition();
		while (pos) {
			CFotografixDoc *pDoc = static_cast<CFotografixDoc *>(OpenDocumentFile(dlg.GetNextPathName(pos)));
			if (readOnly == true) pDoc->hasPath = false;
		}
	}
}

void CFotografixApp::OnFileExtract()
{
	CFileDialog dlg(true, NULL, NULL, OFN_HIDEREADONLY | OFN_ALLOWMULTISELECT | OFN_FILEMUSTEXIST, TEXT("All Files (*.*)|*.*||"), GetMainWnd());

#ifndef _DEBUG
	// Remove MFC hook to enable new Vista dialog
	dlg.m_ofn.Flags &= ~OFN_ENABLEHOOK;
#endif

	TCHAR buffer[4096];
	buffer[0] = 0;
	dlg.m_ofn.lpstrFile = buffer;
	dlg.m_ofn.nMaxFile = 4096;

	if (dlg.DoModal() == IDOK) {
		globals.openExtract = true;

		POSITION pos = dlg.GetStartPosition();
		while (pos)
			OpenDocumentFile(dlg.GetNextPathName(pos));

		globals.openExtract = false;
	}
}

BOOL CFotografixApp::OnIdle(LONG lCount)
{
	if (lCount == 0)
		AfxGetMainWnd()->SendMessage(WM_APP);

	return CWinApp::OnIdle(lCount);
}

void CFotografixApp::OnAppLanguage()
{
	CWnd *wnd = GetMainWnd();
	CFileDialog dlg(true, NULL, NULL, OFN_HIDEREADONLY | OFN_FILEMUSTEXIST, TEXT("Language Files|lang_*.ini||"), wnd);

#ifndef _DEBUG
	// Remove MFC hook to enable new Vista dialog
	dlg.m_ofn.Flags &= ~OFN_ENABLEHOOK;
#endif

	dlg.m_ofn.lpstrInitialDir = globals.appPath;

	if (dlg.DoModal() == IDOK) {
		CString path = dlg.GetPathName();

		if (path.Left(path.ReverseFind('\\')).CompareNoCase(globals.appPath) == 0)
			path = dlg.GetFileName();

		WriteProfileString(TEXT("Window"), TEXT("LanguageFile"), path);
		wnd->MessageBox(LangMessage(InfoRestart), NULL, MB_ICONINFORMATION | MB_OK);
	}
}
