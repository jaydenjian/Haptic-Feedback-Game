#include <Adafruit_NeoPixel.h>

#ifdef __AVR__
  #include <avr/power.h>
#endif
#if defined(__SAM3X8E__) || defined(__SAM3X8H__)
  #define pinNum      8
  #define analogNum   8
  #define NUMPIXELS   8
  const uint8_t pinArray [pinNum] = {2, 3, 4, 5, 6, 7, 8, 9}; //store pin number
  const char lastChar = 'h';
#else
  #define pinNum      4
  #define analogNum   6
  #define NUMPIXELS   4
  const uint8_t pinArray [pinNum] = {5, 6, 9, 10};            //store pin number
  const char lastChar = 'd';
#endif

#define rgbPIN        13
#define ver           1.5
#define rgblight      50      //rgb light
#define highValue     255
#define mediumValue   170
#define lowValue      85

void serialFlush();

uint8_t tag = 1;
uint16_t valueArray [pinNum] = {0};   //store pwm value
char valueType = ' ';

Adafruit_NeoPixel pixels = Adafruit_NeoPixel(NUMPIXELS, rgbPIN, NEO_GRB + NEO_KHZ800);

void setup() {
  // put your setup code here, to run once:

  Serial.begin(9600);

  if(pinNum == 8){
    Serial.println("Board:Arduino Due");
  }else{
    Serial.println("Board:Arduino Uno");
  }
  Serial.print("Version:");
  Serial.println(ver);

  pixels.begin();

}

void loop() {

  uint16_t value = 0, oldvalue = 0;

  while(Serial.available() > 0){
    valueType = Serial.read();
    //Serial.println(valueType);
    if(valueType >= 97 && valueType <= lastChar){
      delay(50);
      while(Serial.available() > 0){
        if(tag){
          tag = 0;
          oldvalue = valueArray[valueType - 97];
          value = 0;
        }
        char tmpValue = Serial.read();
        //Serial.println(tmpValue);
        if(tmpValue >= 48 && tmpValue <= 57){
          value = value * 10 + (tmpValue - 48);
        }else if(tmpValue == 'h'){
          value = highValue;
        }else if(tmpValue == 'm'){
          value = mediumValue;
        }else if(tmpValue == 'l'){
          value = lowValue;
        }else if(tmpValue == 113){
          break;
        }else{
          Serial.println("PWM Value Error");
          serialFlush();
          value = oldvalue;
          break;
        }
      }
    }else if(valueType == 'x'){
      for(int i = 0; i < pinNum; i++)
        valueArray[i] = 0;
      Serial.println("Reset");
      serialFlush();
      break;
    }else if(valueType == 's'){
      Serial.println("Print all Port value");
      for(uint8_t i = 0; i < pinNum; i++){
        Serial.write(97 + i);
        Serial.println(valueArray[i]);
      }
      Serial.println("Done");
      serialFlush();
      break;
    }else if(valueType == 'n'){      
      for(uint8_t i = 0; i < analogNum; i++){
        Serial.write(97 + i);
        Serial.print(analogRead(i), DEC);
        Serial.print("q");
      }
      Serial.println("");
      serialFlush();
      break;
    }else if(valueType == 't'){
      Serial.println("Full Charge");
      for(uint8_t i = 0; i < pinNum; i++)
        valueArray[i] = 255;
      serialFlush();
      break;
    }else{
      Serial.println("Value Type Error");
      serialFlush();
      break;
    }

    if(value <= 255 && value >= 0){
      valueArray[valueType - 97] = value;
      //Serial.write(valueType);
      //Serial.println(valueArray[valueType - 97]);
    }else{
      Serial.println("PWM Value too large");
    }

    tag = 1;

  }

  tag = 1;
  valueType = ' ';

  for(int pwmoutput = 0; pwmoutput < pinNum; pwmoutput++){
    analogWrite(pinArray[pwmoutput], valueArray[pwmoutput]);
    int8_t temp = pwmoutput - pinNum;
    if(valueArray[pwmoutput] > 0 && valueArray[pwmoutput] <= lowValue){
      pixels.setPixelColor(abs(temp) - 1, pixels.Color(0, rgblight, 0));
    }else if(valueArray[pwmoutput] > lowValue && valueArray[pwmoutput] <= mediumValue){
      pixels.setPixelColor(abs(temp) - 1, pixels.Color(0, 0, rgblight));
    }else if (valueArray[pwmoutput] > mediumValue){
      pixels.setPixelColor(abs(temp) - 1, pixels.Color(rgblight, 0, 0));
    }else{
      pixels.setPixelColor(abs(temp) - 1, pixels.Color(0, 0, 0));
    }
//    pixels.setPixelColor(abs(temp) - 1, pixels.Color(0, 0, valueArray[pwmoutput]));
    pixels.show();
  }

}

void serialFlush(){
   delay(1);
  while(Serial.available() > 0){
     delay(1);
    char typeError = Serial.read();
  }
}
