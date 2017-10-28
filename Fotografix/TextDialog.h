#pragma once
#include "afxwin.h"
#include "ColorPicker.h"


// CTextDialog dialog

class CTextDialog : public CDialog
{
	DECLARE_DYNAMIC(CTextDialog)

public:
	CTextDialog(CWnd* pParent = NULL);   // standard constructor
	virtual ~CTextDialog();

// Dialog Data
	enum { IDD = IDD_TEXT };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support

	DECLARE_MESSAGE_MAP()

private:
	CFont font;
	CColorPicker colorPicker;
	CEdit edit;

public:
	int opacity;
	int mode;
	LOGFONT logFont;
	COLORREF color;
	BOOL aa;
	CString text;

public:
	afx_msg void OnBnClickedChoose();
	virtual BOOL OnInitDialog();
	afx_msg void OnStnClickedColor();
	afx_msg HBRUSH OnCtlColor(CDC* pDC, CWnd* pWnd, UINT nCtlColor);
};
