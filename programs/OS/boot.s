# comment
.entry
    movi sp, 0xff
    movi r18, text               # Store the location of the start of the string
    movi t1, print              # Load location of print
    jalr ra, t1                 # Perform print call

.hang
    brc r0, r0, eq, hang

.text
    @ascii "Hello!"

@include C:\\Users\\zache\\Documents\\Coding\\CPU\\OppoT2\\programs\\OS\\routines\\print.s

@org 0x1fff

.intServiceRoutine
    addi s3, r0, 0b11111111111111110
    addi s4, r0, 0b11111111111111111
    lw s5, s3, 0                    
    sw s5, s4, 0                    
    isrr

