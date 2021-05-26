# Arduino_NorFlash_Reader (NorFlash資料下載器)

## 讀取 MXIC / WINBOND Serial NOR Flash

本專案分為兩部分
1. 用Arduino讀取Flash，利用串列將資料報給PC
2. 用C#製作一個PC軟體，讀取串列資料，儲存成檔案

## Wiring  
![image](https://github.com/Chihhao/Arduino_NorFlash_Reader/blob/main/image/wiring_8SOP.png)
![image](https://github.com/Chihhao/Arduino_NorFlash_Reader/blob/main/image/wiring_16SOP.png)

## 材料
(1) Arduino Nano  
(2) MXIC / WINBOND Serial NOR Flash  

## 函式庫
(1) 請複製library目錄中的所有資料到Arduino的library目錄 (ex: C:\Users\USERNAME\Documents\Arduino\libraries)  
(2) 或者可以自行下載，網址如下  
https://github.com/Marzogh/SPIMemory  

## C# UI
![image](https://github.com/Chihhao/Arduino_NorFlash_Reader/blob/main/image/1.png)
![image](https://github.com/Chihhao/Arduino_NorFlash_Reader/blob/main/image/2.png)
![image](https://github.com/Chihhao/Arduino_NorFlash_Reader/blob/main/image/3.png)
