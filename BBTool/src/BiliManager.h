#ifndef BILIMANAGER_H
#define BILIMANAGER_H

#include <QObject>

class BiliManagerPrivate;

class BiliManager : public QObject {
    Q_OBJECT
public:
    explicit BiliManager(QObject *parent = nullptr);
    ~BiliManager();

public:
    void login();
    void logout();

    bool isLogin() const;

protected:
    QScopedPointer<BiliManagerPrivate> d_ptr;

private:
    static BiliManager *self;

public:
    static BiliManager *instance();

signals:
};

#endif // BILIMANAGER_H
