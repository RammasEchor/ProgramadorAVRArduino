; Include definitions
.INCLUDE "m328Pdef.inc"
.EQU LIM_DER = 5
.EQU LIM_IZQ = 0
.EQU DER = 0
.EQU IZQ = 1
.EQU DIR_XOR = 1

; Configuración del sistema
LDI r16, 0xFF
OUT DDRD, r16
OUT DDRB, r16
LDI r16, 0x00
OUT DDRC, r16

; Conf. inicial
LDI r16, 0x01
LDI r17, 0x00
LDI r21, 0x00 ; Banderas
LDI r22, DIR_XOR ; Constante dir

; Main
MAIN:   
    CALL MOSTRAR
    CALL ROTA
    SBRC r17, LIM_DER
        CALL CAMB_DIR
    SBRC r16, LIM_IZQ
        CALL CAMB_DIR
    CALL CHECK_PUSH
    CALL DELAY_500MS
    JMP MAIN

CHECK_PUSH:
    IN r23, PINC
    SBRC r23, 0
        CALL DETENER
    RET

DETENER:
    IN r23, PINC
    ANDI r23, 0b0000_0001
    CPI r23, 0x01
    BREQ DETENER
DETENIDO:
    IN r23, PINC
    ANDI r23, 0b0000_0001
    CPI r23, 0x00
    BREQ DETENIDO
DEJAR:
    IN r23, PINC
    ANDI r23, 0b0000_0001
    CPI r23, 0x01
    BREQ DEJAR
    RET

ROTA:
    SBRC r21, 0
        CALL ROTA_IZQ
    SBRS r21, 0
        CALL ROTA_DER 
    RET

ROTA_DER:
    ROL r16
    ROL r17
    RET

ROTA_IZQ:
    ROR r16
    ROR r17
    RET

MOSTRAR:
    OUT PORTD, r16
    OUT PORTB, r17
    RET

CAMB_DIR:
    EOR r21, r22
    RET

DELAY_500MS:
    ldi  r18, 41
    ldi  r19, 150
    ldi  r20, 128
L1: dec  r20
    brne L1
    dec  r19
    brne L1
    dec  r18
    brne L1
    RET




