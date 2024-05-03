# comment
.entrypoint
    # loc 0
    movi sp, 0xff   # Initialize the stack pointer at location 
    movi r29, code  # Load code location into a register
    jalr r0, r29    # Long jump to .code

@fillto 0x1fff

.intServiceRoutine
    addi s3, r0, 0b11111111111111110
    addi s4, r0, 0b11111111111111111
    lw s5, s3, 0                    
    sw s5, s4, 0                    
    isrr

@fillto 0x3000

.code
    movi r18, text               # Store the location of the start of the string
    movi t1, print              # Load location of print
    jalr ra, t1                 # Perform print call

.hang
    # loc 7
    brc r0, r0, eq, hang # 8191

.text
    # loc 8
    @ascii "Welcome to OppoOS (OppoT2)!"

.routines
    @include C:\\Users\\zache\\Documents\\Coding\\CPU\\OppoT2\\programs\\OS\\routines\\print.s
    @include C:\Users\zache\Documents\Coding\CPU\OppoT2\programs\OS\routines\saveReg.s

