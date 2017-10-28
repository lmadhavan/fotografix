#pragma once


// CColorPicker

class CColorPicker : public CStatic
{
	DECLARE_DYNAMIC(CColorPicker)

public:
	CColorPicker();
	virtual ~CColorPicker();

public:
	COLORREF GetColor() {
		return color;
	}

	void SetColor(COLORREF newColor);

protected:
	DECLARE_MESSAGE_MAP()

private:
	COLORREF color;
	HBRUSH brush;

public:
	afx_msg HBRUSH CtlColor(CDC* /*pDC*/, UINT /*nCtlColor*/);
	afx_msg void OnStnClicked();
};


