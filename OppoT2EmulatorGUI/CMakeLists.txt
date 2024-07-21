cmake_minimum_required(VERSION 3.29)
project(OppoT2EmulatorGUI C)
set(CMAKE_C_STANDARD 99)

find_package(PkgConfig REQUIRED)
pkg_check_modules(GTK4 REQUIRED gtk4)
include_directories(${GTK4_INCLUDE_DIRS})
link_directories(${GTK4_LIBRARY_DIRS})
add_definitions(${GTK4_CFLAGS_OTHER})
set(LIBRARIES ${LIBRARIES} ${GTK4_LIBRARIES})


add_executable(OppoT2EmulatorGUI main.c
        emulator/memory.c
        emulator/interface.c
        emulator/instruction.c
        emulator/emulator.c
        emulator/iocontrol.c
        emulator/headers/memmap.h
)

target_include_directories(OppoT2EmulatorGUI PUBLIC headers)
target_include_directories(OppoT2EmulatorGUI PUBLIC emulator/headers)

target_link_libraries(OppoT2EmulatorGUI ${LIBRARIES})

message(STATUS "GTK include directories: ${GTK4_INCLUDE_DIRS}")
