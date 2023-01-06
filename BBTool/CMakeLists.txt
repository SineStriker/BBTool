include(Linguist.cmake)

cmake_minimum_required(VERSION 3.16)
project(BBTool LANGUAGES CXX VERSION 0.0.0.1)

set(CMAKE_CXX_STANDARD 17)

set(CMAKE_AUTOUIC ON)
set(CMAKE_AUTOMOC ON)
set(CMAKE_AUTORCC ON)

find_package(QT NAMES Qt6 Qt5 COMPONENTS Widgets REQUIRED)
find_package(Qt${QT_VERSION_MAJOR} COMPONENTS Widgets REQUIRED)
list(APPEND _qt_libs Qt${QT_VERSION_MAJOR}::Widgets)

find_package(QT NAMES Qt6 Qt5 COMPONENTS Network REQUIRED)
find_package(Qt${QT_VERSION_MAJOR} COMPONENTS Network REQUIRED)
list(APPEND _qt_libs Qt${QT_VERSION_MAJOR}::Network)

# Add files
file(GLOB_RECURSE _src src/*.h src/*.cpp)

# Add executable
add_executable(${PROJECT_NAME} ${_src} res/res.qrc)

# Add macros
target_compile_definitions(${PROJECT_NAME} PRIVATE
        APP_NAME="${PROJECT_NAME}"
        APP_VERSION="${PROJECT_VERSION}"
        )

# Link libraries
target_link_libraries(${PROJECT_NAME} ${_qt_libs})

# Include directories
target_include_directories(${PROJECT_NAME} PRIVATE src)

# Add translations
target_create_translations(${PROJECT_NAME} SOURCES ${_src}
        LOCALES zh_CN
        DESTINATION res/translations
        CREATE_ONCE
        )

if (WIN32)
    # configure rc
    set(WIN32_ICON_NAME "app.ico")
    set(WIN32_EXPORT_NAME ${PROJECT_NAME})
    set(WIN32_COPYRIGHT_START_YEAR "2022")
    set(WIN32_COPYRIGHT_END_YEAR "2022")
    set(WIN32_AUTHOR_NAME "Sine Striker")
    set(WIN32_FILE_DESC ${PROJECT_NAME})
    set(WIN32_PRODUCT_NAME ${PROJECT_NAME})
    configure_file(
            res/win32/Win32ExeRes.rc.in
            ${CMAKE_CURRENT_BINARY_DIR}/res.rc
            @ONLY
    )

    # copy ico
    file(COPY res/win32/app.ico DESTINATION ${CMAKE_CURRENT_BINARY_DIR})

    # add files
    target_sources(${PROJECT_NAME} PRIVATE
            ${CMAKE_CURRENT_BINARY_DIR}/res.rc
            )
endif ()
