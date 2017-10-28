// MainFrm.h : interface of the CMainFrame class
//


#pragma once

#include "ColorPicker.h"
#include "StaticHeader.h"

class CMainFrame : public CMDIFrameWnd
{
	DECLARE_DYNAMIC(CMainFrame)
public:
	CMainFrame();

// Attributes
public:

// Operations
public:

// Overrides
public:
	virtual BOOL PreCreateWindow(CREATESTRUCT& cs);

// Implementation
public:
	virtual ~CMainFrame();
#ifdef _DEBUG
	virtual void AssertValid() const;
	virtual void Dump(CDumpContext& dc) const;
#endif

protected:  // control bar embedded members
	CFont		m_font;
	CStatusBar  m_wndStatusBar;
	CToolBar    m_wndToolBar;
	CToolBar	m_wndToolsBar;
	CDialogBar	m_wndColor;
	CDialogBar	m_wndSelection;
	CDialogBar	m_wndWand;
	CDialogBar	m_wndLayers;
	CDialogBar	m_wndScripts;
	CDialogBar	m_wndBrush;
	CDialogBar	m_wndGradient;
	CDialogBar	m_wndDummy;

	CDialogBar	*m_pBar;

	CComboBox m_selStyle;
	CComboBox m_brush;
	CComboBox m_brushMode;
	CComboBox m_gradColor;
	CComboBox m_gradType;

	CStaticHeader m_header[5];

	CString brushPath;

// Generated message map functions
protected:
	void DockControlBarEx(CControlBar* pBar, UINT nDockBarID, CControlBar *pPrev);
	afx_msg int OnCreate(LPCREATESTRUCT lpCreateStruct);
	afx_msg void OnDestroy();
	DECLARE_MESSAGE_MAP()

public:
	afx_msg void OnViewFullscreen();
	afx_msg void OnUpdateViewFullscreen(CCmdUI *pCmdUI);

	afx_msg void OnDefaultColors();
	afx_msg void OnSwapColors();
	afx_msg void OnChangeColor();

	afx_msg void OnSelchangeSelstyle();
	afx_msg void OnChangeSelD();
	afx_msg void OnUpdateSelD(CCmdUI *pCmdUI);

	afx_msg void OnChangeTolerance();
	afx_msg void OnContiguous();

	afx_msg void OnSelchangeBrush();
	afx_msg void OnChangeBrushOpacity();
	afx_msg void OnSelchangeBrushMode();

	afx_msg void OnSelchangeGradcolor();
	afx_msg void OnSelchangeGradtype();

	afx_msg void OnLayerNotify(NMHDR *pNMHDR, LRESULT *pResult);
	afx_msg void OnSelchangeChannel();

	afx_msg void OnScriptNotify(NMHDR *pNMHDR, LRESULT *pResult);

	afx_msg void OnSelectTool(UINT nID);
	afx_msg void OnUpdateTool(CCmdUI *pCmdUI);

	afx_msg void OnWindowColour();
	afx_msg void OnUpdateWindowColour(CCmdUI *pCmdUI);
	afx_msg void OnWindowLayers();
	afx_msg void OnUpdateWindowLayers(CCmdUI *pCmdUI);
	afx_msg void OnWindowTools();
	afx_msg void OnUpdateWindowTools(CCmdUI *pCmdUI);
	afx_msg void OnWindowScripts();
	afx_msg void OnUpdateWindowScripts(CCmdUI *pCmdUI);

	afx_msg void OnUpdateStatusDummy(CCmdUI *pCmdUI);

	afx_msg LRESULT OnIdle(WPARAM wParam, LPARAM lParam);
	afx_msg BOOL OnToolTipText(UINT nID, NMHDR* pNMHDR, LRESULT* pResult);

	afx_msg void OnNextbrush();
	afx_msg void OnPrevbrush();
	afx_msg void OnHelp();

private:
	void LoadBrushes(CWnd &target, const CString &path);
	void LoadScripts(CTreeCtrl &target, HTREEITEM root, const CString &path);
};
