#include "I2Cdev.h"
#include "MPU6050_6Axis_MotionApps20.h"
#include "Wire.h"

int MONITOR_BAUDRATE = 9600;
char READ_DELIMITOR = '\n';
int LED_BRIGHTNESS = 100;
int LED_DEATH_DELAY = 150;

int TONE_HIT_DELAY = 500;
int TONE_HIT_FREQUENCY = 100;
int TONE_DEATH_DELAY[3] = {500, 500, 800};
int TONE_DEATH_FREQUENCY[3] = {100, 80, 60};
int TONE_WIN_DELAY[3] = {400, 150, 600};
int TONE_WIN_FREQUENCY[3] = {300, 250, 500};

int DASH_BUTTON_PIN = 15;
int VOLUME_BUTTON_PIN = 14;
int TONE_GENERATOR_PIN = 10;
int HP_LED1_PIN = 11;
int HP_LED2_PIN = 10;
int HP_LED3_PIN = 9;
int HP_DISPLAY_DIGIT_PINS[4] = {5, 3, 2, 4};
int HP_DISPLAY_A_PIN = 6;
int HP_DISPLAY_B_PIN = 7;
int HP_DISPLAY_C_PIN = 8;
int HP_DISPLAY_D_PIN = 9;
int HP_DISPLAY_E_PIN = 13;
int HP_DISPLAY_F_PIN = 11;
int HP_DISPLAY_G_PIN = 12;

auto MPU_6050 = 0x68;
auto ACCEL_START_REGISTER = 0x6B;
auto ACCEL_XOUT_LEFT = 0x3B;
auto ACCEL_XOUT_RIGHT = 0x3C;
auto ACCEL_YOUT_LEFT = 0x3D;
auto ACCEL_YOUT_RIGHT = 0x3E;
auto ACCEL_ZOUT_LEFT = 0x3F;
auto ACCEL_ZOUT_RIGHT = 0x40;
auto GYRO_XOUT_LEFT = 0x43;
auto GYRO_XOUT_RIGHT = 0x44;
auto GYRO_YOUT_LEFT = 0x45;
auto GYRO_YOUT_RIGHT = 0x46;
auto GYRO_ZOUT_LEFT = 0x47;
auto GYRO_ZOUT_RIGHT = 0x48;

int hp = 3;
int volume = 255;

MPU6050 mpu;
// MPU control/status vars
bool dmpReady = false;  // set true if DMP init was successful
uint8_t mpuIntStatus;   // holds actual interrupt status byte from MPU
uint8_t devStatus;      // return status after each device operation (0 = success, !0 = error)
uint16_t packetSize;    // expected DMP packet size (default is 42 bytes)
uint16_t fifoCount;     // count of all bytes currently in FIFO
uint8_t fifoBuffer[64]; // FIFO storage buffer

// orientation/motion vars
Quaternion q;           // [w, x, y, z]         quaternion container
VectorInt16 aa;         // [x, y, z]            accel sensor measurements
VectorInt16 aaReal;     // [x, y, z]            gravity-free accel sensor measurements
VectorInt16 aaWorld;    // [x, y, z]            world-frame accel sensor measurements
VectorFloat gravity;    // [x, y, z]            gravity vector
float euler[3];         // [psi, theta, phi]    Euler angle container
float ypr[3];           // [yaw, pitch, roll]   yaw/pitch/roll container and gravity vector

// packet structure for InvenSense teapot demo
uint8_t teapotPacket[14] = { '$', 0x02, 0,0, 0,0, 0,0, 0,0, 0x00, 0x00, '\r', '\n' };

volatile bool mpuInterrupt = false;     // indicates whether MPU interrupt pin has gone high
void dmpDataReady() {
    mpuInterrupt = true;
}

void setup() {
  Serial.begin(MONITOR_BAUDRATE);
  Serial.setTimeout(1);
  Wire.begin();
  Wire.setClock(400000); // 400kHz I2C clock. Comment this line if having compilation difficulties

  mpu.initialize();
  /*Serial.println(F("Testing device connections..."));
  Serial.println(mpu.testConnection() ? F("MPU6050 connection successful") : F("MPU6050 connection failed"));*/

  devStatus = mpu.dmpInitialize();

  // supply your own gyro offsets here, scaled for min sensitivity
  mpu.setXGyroOffset(220);
  mpu.setYGyroOffset(76);
  mpu.setZGyroOffset(-85);
  mpu.setZAccelOffset(1788); // 1688 factory default for my test chip

  // make sure it worked (returns 0 if so)
  if (devStatus == 0) {
      // Calibration Time: generate offsets and calibrate our MPU6050
      mpu.CalibrateAccel(6);
      mpu.CalibrateGyro(6);
      mpu.PrintActiveOffsets();
      // turn on the DMP, now that it's ready
      //Serial.println(F("Enabling DMP..."));
      mpu.setDMPEnabled(true);

      // enable Arduino interrupt detection
      /*Serial.print(F("Enabling interrupt detection (Arduino external interrupt "));
      Serial.print(digitalPinToInterrupt(2));
      Serial.println(F(")..."));
      attachInterrupt(digitalPinToInterrupt(2), dmpDataReady, RISING);
      mpuIntStatus = mpu.getIntStatus();*/

      // set our DMP Ready flag so the main loop() function knows it's okay to use it
      //Serial.println(F("DMP ready! Waiting for first interrupt..."));
      dmpReady = true;

      // get expected DMP packet size for later comparison
      packetSize = mpu.dmpGetFIFOPacketSize();
  } else {
      // ERROR!
      // 1 = initial memory load failed
      // 2 = DMP configuration updates failed
      // (if it's going to break, usually the code will be 1)
      //Serial.print(F("DMP Initialization failed (code "));
      //Serial.print(devStatus);
      //Serial.println(F(")"));
  }

  pinMode(DASH_BUTTON_PIN, INPUT_PULLUP);
  pinMode(VOLUME_BUTTON_PIN, INPUT);
  pinMode(TONE_GENERATOR_PIN, OUTPUT);

  /*pinMode(HP_LED1_PIN, OUTPUT);
  pinMode(HP_LED2_PIN, OUTPUT);
  pinMode(HP_LED3_PIN, OUTPUT);*/

  for(int i = 0; i < 4; i++)
    pinMode(HP_DISPLAY_DIGIT_PINS[i], OUTPUT);
  pinMode(HP_DISPLAY_A_PIN, OUTPUT);
  pinMode(HP_DISPLAY_B_PIN, OUTPUT);
  pinMode(HP_DISPLAY_C_PIN, OUTPUT);
  pinMode(HP_DISPLAY_D_PIN, OUTPUT);
  pinMode(HP_DISPLAY_E_PIN, OUTPUT);
  pinMode(HP_DISPLAY_F_PIN, OUTPUT);
  pinMode(HP_DISPLAY_G_PIN, OUTPUT);
}

float accZ = 0;
float angleZ = 0;

String winMessages[2] = {"GG  ", ""};
String deadMessages[2] = {"HP 0", ""};
String gameOverMessages[2] = {"GAME", "OVER"};
bool win = false;
bool dead = false;
bool gameOver = false;
long endNextTime = 0;
int endDelay = 1000;
int endI = 0;
long gameOverNextTime = 0;
int gameOverI = 0;
int deadDelay = 200;

void loop() {
  // if programming failed, don't try to do anything
  //if (!dmpReady) return;

  bool dash = digitalRead(DASH_BUTTON_PIN) == 1;
  volume = map(analogRead(VOLUME_BUTTON_PIN), 0, 1023, 0, 255);

  if (mpu.dmpGetCurrentFIFOPacket(fifoBuffer)) {
    angleZ = getAngleZ();
    accZ = getAccZ();
  }

  sendMessage(volume, dash, accZ, angleZ);

  int life = getMessage();

  if(win || gameOver) {
    if(millis() >= endNextTime) {
      endI = (endI + 1) % 2;
      endNextTime = millis() + (gameOver && dead ? deadDelay : endDelay);
    }

    if(win) {
      displayMessage(winMessages[endI]);

      if(millis() >= gameOverNextTime) {
        if(gameOverI > 2) {
          noTone(TONE_GENERATOR_PIN);
        } else if(gameOverI <= 2) {
          gameOverI++;

          if(gameOverI <= 2) {
            gameOverNextTime = millis() + TONE_WIN_DELAY[gameOverI];
            tone(TONE_GENERATOR_PIN, TONE_WIN_FREQUENCY[gameOverI], TONE_WIN_DELAY[gameOverI]);
          }
        }
      }
    }
    else {
      if(dead) displayMessage(deadMessages[endI]);
      else displayMessage(gameOverMessages[endI]);

      if(millis() >= gameOverNextTime) {
        if(gameOverI > 2 && dead) {
          dead = false;
          endI = 0;
          endNextTime = millis() + endDelay;
          noTone(TONE_GENERATOR_PIN);
        } else if(gameOverI <= 2) {
          gameOverI++;

          if(gameOverI <= 2) {
            gameOverNextTime = millis() + TONE_DEATH_DELAY[gameOverI];
            tone(TONE_GENERATOR_PIN, TONE_DEATH_FREQUENCY[gameOverI], TONE_DEATH_DELAY[gameOverI]);
          }
        }
      }
    }
  }

  if(life >= 0) {
    if(life < hp)
      onDamageTaken(life);

    hp = life;
  }

  if(hp > 0 && !win && !gameOver) displayMessage("HP " + String(hp, DEC));
}

float getAngleZ() {
  mpu.dmpGetQuaternion(&q, fifoBuffer);
  mpu.dmpGetGravity(&gravity, &q);
  mpu.dmpGetYawPitchRoll(ypr, &q, &gravity);

  return ypr[2] * 180/M_PI * -1;
}

float getAccZ() {
  mpu.dmpGetQuaternion(&q, fifoBuffer);
  mpu.dmpGetAccel(&aa, fifoBuffer);
  mpu.dmpGetGravity(&gravity, &q);
  mpu.dmpGetLinearAccel(&aaReal, &aa, &gravity);
  mpu.dmpGetLinearAccelInWorld(&aaWorld, &aaReal, &q);

  return aaWorld.z;
}

void onDamageTaken(int life) {
  volume = 255;
  if(hp > 0 && life == 0) {
      gameOver = true;
      dead = true;
      endNextTime = millis() + deadDelay;
      endI = 0;
      gameOverI = -1;
      gameOverNextTime = millis();
    return;
  }
  
  tone(TONE_GENERATOR_PIN, TONE_HIT_FREQUENCY, TONE_HIT_DELAY);
}

int charTableLen = 21;
int charTable[21][9] {
  {'0', HIGH, HIGH, HIGH, HIGH, HIGH, HIGH, LOW},
  {'1', LOW, HIGH, HIGH, LOW, LOW, LOW, LOW},
  {'2', HIGH, HIGH, LOW, HIGH, HIGH, LOW, HIGH},
  {'3', HIGH, HIGH, HIGH, HIGH, LOW, LOW, HIGH},
  {'A', HIGH, HIGH, HIGH, LOW, HIGH, HIGH, HIGH},
  {'D', HIGH, HIGH, HIGH, HIGH, HIGH, HIGH, LOW},
  {'E', HIGH, LOW, LOW, HIGH, HIGH, HIGH, HIGH},
  {'G', HIGH, LOW, HIGH, HIGH, HIGH, HIGH, LOW},
  {'H', LOW, HIGH, HIGH, LOW, HIGH, HIGH, HIGH},
  {'I', LOW, HIGH, HIGH, LOW, LOW, LOW, LOW},
  {'M', HIGH, HIGH, HIGH, LOW, HIGH, HIGH, LOW},
  {'N', HIGH, HIGH, HIGH, LOW, HIGH, HIGH, LOW},
  {'O', HIGH, HIGH, HIGH, HIGH, HIGH, HIGH, LOW},
  {'P', HIGH, HIGH, LOW, LOW, HIGH, HIGH, HIGH},
  {'R', HIGH, LOW, LOW, LOW, HIGH, HIGH, LOW},
  {'U', LOW, HIGH, HIGH, HIGH, HIGH, HIGH, LOW},
  {'V', LOW, HIGH, HIGH, HIGH, HIGH, HIGH, LOW},
  {'<', LOW, LOW, HIGH, HIGH, HIGH, HIGH, LOW},
  {'>', LOW, HIGH, HIGH, HIGH, HIGH, LOW, LOW},
  {'Y', LOW, HIGH, HIGH, LOW, LOW, HIGH, HIGH},
  {' ', LOW, LOW, LOW, LOW, LOW, LOW, LOW}
};

void displayMessage(String message) {
    int len = message.length();
    char c1 = ' ';
    char c2 = ' ';
    char c3 = ' ';
    char c4 = ' ';

    if(len > 0) c1 = message.charAt(0);
    if(len > 1) c2 = message.charAt(1);
    if(len > 2) c3 = message.charAt(2);
    if(len > 3) c4 = message.charAt(3);

    displayChar(c1, 0);
    delay(3);
    displayChar(c2, 1);
    delay(3);
    displayChar(c3, 2);
    delay(3);
    displayChar(c4, 3);
    delay(3);

    
}

void displayToggle(int display, bool on) {
  digitalWrite(display, on ? LOW : HIGH);
}

void displayChar(char c, int display) {
  int* table;

  for(int i = 0; i < charTableLen; i++) {
    if(charTable[i][0] == c) {
      table = charTable[i];
    }
  }

  for(int i = 0; i < 4; i++)
    displayToggle(HP_DISPLAY_DIGIT_PINS[i], display == i);

  digitalWrite(HP_DISPLAY_A_PIN, table[1]);
  digitalWrite(HP_DISPLAY_B_PIN, table[2]);
  digitalWrite(HP_DISPLAY_C_PIN, table[3]);
  digitalWrite(HP_DISPLAY_D_PIN, table[4]);
  digitalWrite(HP_DISPLAY_E_PIN, table[5]);
  digitalWrite(HP_DISPLAY_F_PIN, table[6]);
  digitalWrite(HP_DISPLAY_G_PIN, table[7]);
}

void startAccelerometer() {
  Wire.beginTransmission(MPU_6050);
  Wire.write(ACCEL_START_REGISTER);
  Wire.write(0);
  Wire.endTransmission();
}

void requestAccelerometer(int memPos) {
  Wire.beginTransmission(MPU_6050);
  Wire.write(memPos);
  Wire.endTransmission();
  Wire.requestFrom(MPU_6050, 2);
}

void sendMessage(int volume, bool dash, float accZ, float angleZ) {
  Serial.print(volume);
  Serial.print(" ");
  Serial.print(dash);
  Serial.print(" ");
  Serial.print(accZ);
  Serial.print(" ");
  Serial.print(angleZ);
  Serial.println();
}

int getMessage() {
  String read = Serial.readStringUntil(READ_DELIMITOR);

  if(read.compareTo("S") == 0) {
    hp = 3;
    win = false;
    gameOver = false;
    dead = false;
    return -2;
  }

  if(read.compareTo("W") == 0) {
    win = true;
    endI = 0;
    endNextTime = millis() + endDelay;
    gameOverI = -1;
    gameOverNextTime = millis();
    return -3;
  }

  return read.length() > 0 ? read.toInt() : -1;
}
