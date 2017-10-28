#pragma once


// CStaticHeader

class CStaticHeader : public CStatic
{
	DECLARE_DYNAMIC(CStaticHeader)

public:
	CStaticHeader();
	virtual ~CStaticHeader();

protected:
	DECLARE_MESSAGE_MAP()

public:
	afx_msg HBRUSH CtlColor(CDC* /*pDC*/, UINT /*nCtlColor*/);
};


