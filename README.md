
# OppoT2

OppoT2 is a RISC CPU designed under the Von Neumann architecture. The CPU is a 32-bit sequential design with stack and simple interrupt support. This repository contains a syntax highlighter for [VSCode](https://code.visualstudio.com/), an assembler for the CPU, the [Logisim Evolution](https://github.com/logisim-evolution/logisim-evolution) file that contains my implementation of the CPU, and various programs designed to run on the processor.


## Features

### CPU
- Stack and Interrupt Support
- Memory Mapped I/O
- 32-bit Instruction Width and Address Width

### Assembler
- Support for simple directives (fillto, ascii)
- Label Support
- Multifile Inclusion


## Installation

Run ```git clone https://github.com/ZacheryCalahan/OppoT2.git``` to get a local copy of the repository for the full source, or download the latest release from Github.

    
## Usage

### CPU
Install [Logisim Evolution](https://github.com/logisim-evolution/logisim-evolution) and open the OppoT2.circ file.

### Assembler

Run the assembler executable in the Assembler.zip file in the latest release in the command line.

```
./OppoT2Assembler.exe {Assembly entry file} {Name of binary to output}
```

This outputs a text file that contains hex that can be entered into the RAM module in Logisim.

### Syntax Highlighter
Install the .vsix extention into VSCode, and select the OppoT2 language in your editor.




## Documentation

Documentation is currently in the works, but will define the following:

- ISA
- Psuedo-operands
- Assembler usage
- Ideal Memory Map


## Acknowledgements

The design of the processor is built upon a simple design from Bruce Jacob at the University of Maryland called [RiSC-16](https://user.eng.umd.edu/~blj/RiSC/). Massive thanks to Prof. Jacob for providing the documentation that taught me how CPUs work.
 

