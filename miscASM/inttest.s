.start
    # Address of TTY
    addi s0, r0, 0x1FFFF         

    # Write Chars
    addi s1, r0, 0b01001000 # H  
    sw s1, s0, 0             

    addi s1, r0, 0b01100101 # e  
    sw s1, s0, 0             

    addi s1, r0, 0b01101100 # l x
    sw s1, s0, 0             
    sw s1, s0, 0             

    addi s1, r0, 0b01101111  # o 
    sw s1, s0, 0             

    addi s1, r0, 0b00100000 # spa
    sw s1, s0, 0             

    addi s1, r0, 0b01010111 # W  
    sw s1, s0, 0             

    addi s1, r0, 0b01101111 # o  
    sw s1, s0, 0             

    addi s1, r0, 0b01110010# r   
    sw s1, s0, 0             

    addi s1, r0, 0b01101100 # l  
    sw s1, s0, 0             

    addi s1, r0, 0b01100100# d   
    sw s1, s0, 0             

    addi s1, r0, 0b00100001# !   
    sw s1, s0, 0             

    brc r0, r0, EQ, 0x1FFF       

.hang
    brc r0, r0, EQ, hang

@fillto 0x2f

.intServiceRoutine
    addi s3, r0, 0b11111111111111110
    addi s4, r0, 0b11111111111111111
    lw s5, s3, 0                    
    sw s5, s4, 0                    
    isrr