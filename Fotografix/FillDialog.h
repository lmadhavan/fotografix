#pragma once


// CFillDialog dialog

class CFillDialog : public CDialog
{
	DECLARE_DYNAMIC(CFillDialog)

public:
	CFillDialog(CWnd* pParent = NULL);   // standard constructor
	virtual ~CFillDialog();

// Dialog Data
	enum { IDD = IDD_FILL };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support

	DECLARE_MESSAGE_MAP()

public:
	int colour;
	int opacity;
public:
	virtual BOOL OnInitDialog();
};
