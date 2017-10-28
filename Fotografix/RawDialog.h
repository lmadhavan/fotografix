#pragma once


// CRawDialog dialog

class CRawDialog : public CDialog
{
	DECLARE_DYNAMIC(CRawDialog)

public:
	CRawDialog(CWnd* pParent = NULL);   // standard constructor
	virtual ~CRawDialog();

// Dialog Data
	enum { IDD = IDD_RAWIMPORT };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support

	DECLARE_MESSAGE_MAP()

public:
	__int64 length;
	CString size;
	int w;
	int h;
	int header;

public:
	virtual BOOL OnInitDialog();
	afx_msg void OnBnClickedOk();
};
