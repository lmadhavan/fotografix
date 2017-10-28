#pragma once


// CLayerDialog dialog

class CLayerDialog : public CDialog
{
	DECLARE_DYNAMIC(CLayerDialog)

public:
	CLayerDialog(CWnd* pParent = NULL);   // standard constructor
	virtual ~CLayerDialog();

// Dialog Data
	enum { IDD = IDD_LAYER };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support

	DECLARE_MESSAGE_MAP()

public:
	int opacity;
public:
	int mode;
public:
	virtual BOOL OnInitDialog();
};
