#include <QApplication>

#include "MainWindow.h"

int main(int argc, char *argv[]) {
    QApplication a(argc,argv);
    a.setApplicationName(APP_NAME);
    a.setApplicationVersion(APP_VERSION);

    MainWindow w;
    w.show();

    return a.exec();
}
