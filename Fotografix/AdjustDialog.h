#pragma once
#include "afxcmn.h"

class CFotografixDoc;

// CAdjustDialog dialog

class CAdjustDialog : public CDialog
{
	DECLARE_DYNAMIC(CAdjustDialog)

public:
	CAdjustDialog(CWnd* pParent = NULL);   // standard constructor
	virtual ~CAdjustDialog();

// Dialog Data
	enum { IDD = IDD_ADJUST };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support

	DECLARE_MESSAGE_MAP()

public:
	int num;
	int min[3], max[3], def[3];
	CString title;

	BOOL preview;

	bool adjustLayer;
	int adjustType;
	int layer;
	int channelMask;
	CFotografixDoc *pDoc;

	CString label[3];
	int value[3];
	CSliderCtrl slider[3];

private:
	bool change;
	bool adjust;

public:
	virtual BOOL OnInitDialog();
	afx_msg void OnHScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar);
	afx_msg void OnEnChangeEdit1();
	afx_msg void OnEnChangeEdit2();
	afx_msg void OnEnChangeEdit3();
	afx_msg void OnTimer(UINT_PTR nIDEvent);
	afx_msg void OnBnClickedOk();
	afx_msg void OnBnClickedCancel();
	afx_msg void OnBnClickedPreview();
	afx_msg void OnBnClickedReset1();
	afx_msg void OnBnClickedReset2();
	afx_msg void OnBnClickedReset3();
};
