#ifndef MAINWINDOW_H
#define MAINWINDOW_H

#include <QLabel>
#include <QLineEdit>
#include <QMainWindow>
#include <QVBoxLayout>
#include <QPushButton>
#include <QThread>

#include "CNavFrame.h"

class MainWindow : public QMainWindow {
    Q_OBJECT
public:
    explicit MainWindow(QWidget *parent = nullptr);
    ~MainWindow();

public:
    QVBoxLayout *mainLayout;
    QHBoxLayout *topLayout;

    QPushButton *userButton;
    CNavFrame *navFrame;

    QPushButton *aboutButton;

private:
    void _q_userButtonClicked();
    void _q_aboutButtonClicked();

signals:
};

#endif // MAINWINDOW_H
