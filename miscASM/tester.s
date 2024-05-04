.start
    movi s0, print
    movi sp, 0xf00
    jalr ra, s0

.hang
    brc r0, r0, eq, hang

@org 0x1ff
@include C:\Users\zache\Documents\Coding\CPU\OppoT2\miscASM\asciiPrint.s



