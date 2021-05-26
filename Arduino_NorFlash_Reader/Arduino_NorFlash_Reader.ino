#include <SPIMemory.h>
#define LED_RED 2
#define LED_GREEN 3
#define CMD_READ_ALL 60
#define CMD_CLEAR_ALL 61
#define SIGN_START 150
#define SIGN_END 151
#define CONFIG_4MB_16B   101
#define CONFIG_4MB_32B   102
#define CONFIG_4MB_64B   103
#define CONFIG_16MB_16B  104
#define CONFIG_16MB_32B  105
#define CONFIG_16MB_64B  106
#define CONFIG_32MB_16B  107
#define CONFIG_32MB_32B  108
#define CONFIG_32MB_64B  109
#define CONFIG_64MB_16B  110
#define CONFIG_64MB_32B  111
#define CONFIG_64MB_64B  112
#define CONFIG_128MB_16B 113
#define CONFIG_128MB_32B 114
#define CONFIG_128MB_64B 115
           
SPIFlash flash;

uint32_t CAPA=0;
//Capacity: 134217728 Byte
int ARRSZ=32;
char outputArray16[16];
char outputArray32[32];
char outputArray64[64];

void setup() {
  pinMode(12, INPUT_PULLUP);
  pinMode(LED_RED, OUTPUT);
  pinMode(LED_GREEN, OUTPUT);
  digitalWrite(LED_RED, LOW);
  digitalWrite(LED_GREEN, LOW);
  
  Serial.begin(115200);
  while (!Serial) ;
  
  //Serial.println(F("START"));
}

void loop() {
  while (Serial.available() > 0) {
    uint8_t commandNo = Serial.parseInt();
    if (commandNo == CMD_CLEAR_ALL) {
      //Serial.println(F("Function 14 : Erase Chip"));
      //Serial.println(F("Please wait a minute"));      
      digitalWrite(LED_RED, HIGH);
      if (flash.eraseChip()){ 
        Serial.print((char)SIGN_START);      
        Serial.print(F("CleanOK"));
        Serial.print((char)SIGN_END);             
      }
      else{        
        Serial.print((char)SIGN_START);      
        Serial.print(F("CleanNG"));
        Serial.print((char)SIGN_END);
      }
      digitalWrite(LED_RED, LOW);      
    }
    else if (commandNo == CMD_READ_ALL){
      digitalWrite(LED_GREEN, HIGH);
      Serial.println("[FLASH READ START]");
      for(uint32_t adr=0; adr<CAPA; adr+=ARRSZ){
        //myPrintHex(adr);    
        //Serial.print(": ");
        if(flash.readByte(adr)==0xFF){
          //Serial.println("no data");
          //Serial.println("stop");
          break;
        }
        else{
          if (flash.readCharArray(adr, outputArray64, ARRSZ)) {
            Serial.println(outputArray64); }     
          else{
            Serial.println("read fail!"); }  
        }  
      }
      Serial.println("[FLASH READ END]");
      Serial.print((char)SIGN_END);
      digitalWrite(LED_GREEN, LOW);
    }
    else if (commandNo == CONFIG_128MB_16B){ initFlash(CONFIG_128MB_16B); }
    else if (commandNo == CONFIG_128MB_32B){ initFlash(CONFIG_128MB_32B); }
    else if (commandNo == CONFIG_128MB_64B){ initFlash(CONFIG_128MB_64B); }
    else if (commandNo == CONFIG_64MB_16B){ initFlash(CONFIG_64MB_16B); }
    else if (commandNo == CONFIG_64MB_32B){ initFlash(CONFIG_64MB_32B); }
    else if (commandNo == CONFIG_64MB_64B){ initFlash(CONFIG_64MB_64B); }
    else if (commandNo == CONFIG_32MB_16B){ initFlash(CONFIG_32MB_16B); }
    else if (commandNo == CONFIG_32MB_32B){ initFlash(CONFIG_32MB_32B); }
    else if (commandNo == CONFIG_32MB_64B){ initFlash(CONFIG_32MB_64B); }
    else if (commandNo == CONFIG_16MB_16B){ initFlash(CONFIG_16MB_16B); }
    else if (commandNo == CONFIG_16MB_32B){ initFlash(CONFIG_16MB_32B); }
    else if (commandNo == CONFIG_16MB_64B){ initFlash(CONFIG_16MB_64B); }
    else if (commandNo == CONFIG_4MB_16B){ initFlash(CONFIG_4MB_16B); }
    else if (commandNo == CONFIG_4MB_32B){ initFlash(CONFIG_4MB_32B); }
    else if (commandNo == CONFIG_4MB_64B){ initFlash(CONFIG_4MB_64B); }
  }
}

void initFlash(int iConfig){
  switch(iConfig) {
    case CONFIG_4MB_16B:   
      flash.begin(MB(4)); ARRSZ=16; break;    
    case CONFIG_4MB_32B:
      flash.begin(MB(4)); ARRSZ=32; break;    
    case CONFIG_4MB_64B:
      flash.begin(MB(4)); ARRSZ=64; break;  
    //--------------
    case CONFIG_16MB_16B:   
      flash.begin(MB(16)); ARRSZ=16; break;    
    case CONFIG_16MB_32B:
      flash.begin(MB(16)); ARRSZ=32; break;    
    case CONFIG_16MB_64B:
      flash.begin(MB(16)); ARRSZ=64; break; 
    //-------------- 
    case CONFIG_32MB_16B:   
      flash.begin(MB(32)); ARRSZ=16; break;    
    case CONFIG_32MB_32B:
      flash.begin(MB(32)); ARRSZ=32; break;    
    case CONFIG_32MB_64B:
      flash.begin(MB(32)); ARRSZ=64; break;
    //--------------
    case CONFIG_64MB_16B:   
      flash.begin(MB(64)); ARRSZ=16; break;    
    case CONFIG_64MB_32B:
      flash.begin(MB(64)); ARRSZ=32; break;    
    case CONFIG_64MB_64B:
      flash.begin(MB(64)); ARRSZ=64; break;
    //--------------
    case CONFIG_128MB_16B:   
      flash.begin(MB(128)); ARRSZ=16; break;    
    case CONFIG_128MB_32B:
      flash.begin(MB(128)); ARRSZ=32; break;    
    case CONFIG_128MB_64B:
      flash.begin(MB(128)); ARRSZ=64; break;         
  }
  CAPA=flash.getCapacity();
  Serial.print(F("Capacity: ")); 
  Serial.print(CAPA); 
  Serial.println(F(" Byte"));
  Serial.print((char)SIGN_START);      
  Serial.print(F("SettingOK"));
  Serial.print((char)SIGN_END);
  //Serial.println();
}

void myPrintHex(uint32_t inputInt32){
  if(inputInt32>0x0FFFFFFF){
    Serial.print("0x");
    Serial.print(inputInt32, HEX);
  }
  else if (inputInt32>0x00FFFFFF){
    Serial.print("0x0");
    Serial.print(inputInt32, HEX);
  }
  else if (inputInt32>0x000FFFFF){
    Serial.print("0x00");
    Serial.print(inputInt32, HEX);
  }
  else if (inputInt32>0x0000FFFF){
    Serial.print("0x000");
    Serial.print(inputInt32, HEX);
  }
  else if (inputInt32>0x00000FFF){
    Serial.print("0x0000");
    Serial.print(inputInt32, HEX);
  }
  else if (inputInt32>0x000000FF){
    Serial.print("0x00000");
    Serial.print(inputInt32, HEX);
  }
  else if (inputInt32>0x0000000F){
    Serial.print("0x000000");
    Serial.print(inputInt32, HEX);
  }
  else {
    Serial.print("0x0000000");
    Serial.print(inputInt32, HEX);
  }
  
}

void erase4K(unsigned long addr){
  myPrintHex(addr);
  if (flash.eraseSector(addr)) {
    Serial.println(F(" erase 4KB"));
  }
  else {
    Serial.println(F("Erasing sector failed"));
  } 
  //delay(10);
}
