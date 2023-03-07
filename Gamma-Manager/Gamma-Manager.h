
// Gamma-Manager.h: главный файл заголовка для приложения PROJECT_NAME
//

#pragma once

#ifndef __AFXWIN_H__
	#error "включить pch.h до включения этого файла в PCH"
#endif

#include "resource.h"		// основные символы


// CGammaManagerApp:
// Сведения о реализации этого класса: Gamma-Manager.cpp
//

class CGammaManagerApp : public CWinApp
{
public:
	CGammaManagerApp();

// Переопределение
public:
	virtual BOOL InitInstance();

// Реализация

	DECLARE_MESSAGE_MAP()
};

extern CGammaManagerApp theApp;
