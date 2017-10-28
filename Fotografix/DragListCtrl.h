#pragma once

#define LVNEX_FINISHDRAG	(LVN_FIRST-100)

struct NMDRAGINFO {
	NMHDR hdr;
	int fromItem, toItem;
};

// CDragListCtrl

class CDragListCtrl : public CListCtrl
{
	DECLARE_DYNAMIC(CDragListCtrl)

public:
	CDragListCtrl();
	virtual ~CDragListCtrl();

protected:
	DECLARE_MESSAGE_MAP()

private:
	int fromItem;
	int toItem;
	HCURSOR dragCursor;

public:
	afx_msg void OnMouseMove(UINT nFlags, CPoint point);
	afx_msg void OnLButtonUp(UINT nFlags, CPoint point);
	afx_msg void OnLvnBegindrag(NMHDR *pNMHDR, LRESULT *pResult);
};


