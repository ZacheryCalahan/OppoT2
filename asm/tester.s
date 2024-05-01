# Set r3 to 0xFFFFFFFF (test immed)
lui s1, 0x7FFF                      #01110 00011 0000000 111111111111111     0x70C07FFF
addi s1, r0, 0x1FFFF                    #00001 00011 00000 11111111111111111     0x08C1FFFF
