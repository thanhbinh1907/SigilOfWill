@echo off
chcp 65001 > nul
title Bo cong cu dong goi du an Sigil Of Will (Unity + AI)

echo ==============================================================
echo [DONG GOI] BAT DAU DONG GOI DU AN SIGIL OF WILL (UNITY + AI BACKEND)
echo ==============================================================
echo.

:: 1. Chay script Python de dong goi AI bang PyInstaller
echo [BUOC 1] Dong goi AI Backend va cac Models...
python Model_SOW\build_ai.py
if %ERRORLEVEL% neq 0 (
    echo.
    echo [LOI] Co loi xay ra trong qua trinh dong goi AI. Vui long kiem tra lai moi truong Python.
    pause
    exit /b %ERRORLEVEL%
)

echo.
echo [BUOC 2] Tao cau truc thu muc Release...
:: Tao thu muc Release
if not exist "Builds\SigilOfWill_Release" (
    mkdir "Builds\SigilOfWill_Release"
)
if not exist "Builds\SigilOfWill_Release\AI" (
    mkdir "Builds\SigilOfWill_Release\AI"
)

:: 2. Sao chep ket qua dong goi AI vao thu muc Release
echo [BUOC 3] Di chuyen du lieu AI sang thu muc Release...
xcopy /E /I /Y "Model_SOW\dist\AI_Sender_Ges_Voice" "Builds\SigilOfWill_Release\AI" > nul
if %ERRORLEVEL% neq 0 (
    echo [LOI] Co loi khi sao chep thu muc AI sang Builds\SigilOfWill_Release\AI.
    pause
    exit /b %ERRORLEVEL%
)

:: 3. Tao file khoi dong RunGame.bat trong thu muc Release
echo [BUOC 4] Tao file kich ban khoi chay RunGame.bat...
(
echo @echo off
echo chcp 65001 ^> nul
echo title Sigil Of Will - Khoi dong Game va AI
echo.
echo ==============================================================
echo [KHOI DONG] Dang khoi dong Sigil of Will ^(Unity ^+ AI Backend^)...
echo ==============================================================
echo.
echo [AI] 1. Dang khoi chay Bo nhan dien AI ^(Cu chi va Giong noi^)...
echo [HE THONG] Camera va Micro se duoc kich hoat. Vui long giu cua so console nay mo.
echo.
echo Chu y: De giu phim 'E' trong game de niem chu!
echo.
echo --------------------------------------------------------------
echo Khoi chay AI_Sender_Ges_Voice...
echo :: Khoi chay AI_Sender tu thu muc AI ^(dung /D de thiet lap thu muc lam viec va %%~dp0 de lay duong dan tuyet doi^)
echo start "" /D "%%~dp0AI" "%%~dp0AI\AI_Sender_Ges_Voice.exe"
echo.
echo [GAME] 2. Dang khoi chay Game Sigil Of Will...
echo :: Khoi chay Game Unity
echo start "" /D "%%~dp0" "%%~dp0SigilOfWill.exe"
echo.
echo [THANH CONG] DA KICH HOAT THANH CONG! Chuc ban choi game vui ve.
echo.
echo timeout /t 5
) > "Builds\SigilOfWill_Release\RunGame.bat"

echo.
echo ==============================================================
echo [HOAN TAT] QUA TRINH TU DONG DONG GOI AI HOAN TAT!
echo ==============================================================
echo.
echo Duong dan thu muc release cua ban:
echo -^> %~dp0Builds\SigilOfWill_Release
echo.
echo [CAC BUOC CAN LAM TIEP THEO DE HOAN THANH]:
echo 1. Mo Unity Editor len, vao File -^> Build Settings.
echo 2. Chon platform Windows va thuc hien Build game.
echo 3. Khi Unity hoi noi luu, hay chon thu muc:
echo    %~dp0Builds\SigilOfWill_Release
echo    ^(Hoac build ra cho khac roi COPY toan bo file .exe game va cac thu muc kem theo nhu
echo     SigilOfWill_Data, MonoBleedingEdge, UnityPlayer.dll vao thu muc SigilOfWill_Release tren^).
echo.
echo 4. Chay file "RunGame.bat" trong thu muc Release de choi game ket hop AI!
echo.
pause
