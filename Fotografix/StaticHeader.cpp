// StaticHeader.cpp : implementation file
//

#include "stdafx.h"
#include "Fotografix.h"
#include "StaticHeader.h"


// CStaticHeader

IMPLEMENT_DYNAMIC(CStaticHeader, CStatic)

CStaticHeader::CStaticHeader()
{

}

CStaticHeader::~CStaticHeader()
{
}


BEGIN_MESSAGE_MAP(CStaticHeader, CStatic)
	ON_WM_CTLCOLOR_REFLECT()
END_MESSAGE_MAP()



// CStaticHeader message handlers



HBRUSH CStaticHeader::CtlColor(CDC* pDC, UINT /*nCtlColor*/)
{
	pDC->SetBkColor(RGB(128, 128, 128));
	pDC->SetTextColor(RGB(255, 255, 255));

	return (HBRUSH)::GetStockObject(GRAY_BRUSH);
}
