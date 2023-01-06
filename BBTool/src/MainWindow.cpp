#include "MainWindow.h"

#include <QApplication>
#include <QMessageBox>

#include "Panels/CommentsWorkPanel.h"

MainWindow::MainWindow(QWidget *parent) : QMainWindow(parent) {
    userButton = new QPushButton();
    aboutButton = new QPushButton(tr("About"));
    navFrame = new CNavFrame();

    mainLayout = new QVBoxLayout();
    mainLayout->setMargin(0);
    mainLayout->setSpacing(0);

    topLayout = new QHBoxLayout();
    topLayout->setMargin(0);
    topLayout->setSpacing(0);

    topLayout->addWidget(userButton);
    topLayout->addStretch();

    mainLayout->addLayout(topLayout);
    mainLayout->addWidget(navFrame);

    auto w = new QWidget();
    w->setLayout(mainLayout);
    setCentralWidget(w);

    // Initial components
    userButton->setText(tr("No login"));
    userButton->setIcon(QIcon(":/images/akkarin.png"));

    navFrame->setBottomWidget(aboutButton);

    connect(aboutButton, &QPushButton::clicked, this, &MainWindow::_q_aboutButtonClicked);

    auto panel1 = new CommentsWorkPanel();
    auto btn1 = navFrame->addWidget(panel1);
    btn1->setObjectName("comments-work-btn");
    btn1->setText(tr("Comments message sender"));
}

MainWindow::~MainWindow() {
}

void MainWindow::_q_userButtonClicked()
{

}

void MainWindow::_q_aboutButtonClicked() {
    QMessageBox::information(
        this, qAppName(),
        tr("%1 %2, Copyright (c) Sine Striker").arg(qAppName(), qApp->applicationVersion()));
}
