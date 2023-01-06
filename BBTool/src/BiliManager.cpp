#include "BiliManager.h"

BiliManager *BiliManager::self = nullptr;

class BiliManagerPrivate {
public:
    BiliManager *q_ptr;

    bool logged;
};

BiliManager::BiliManager(QObject *parent) : QObject(parent), d_ptr(new BiliManagerPrivate()) {
    self = this;

    d_ptr->logged = false;
}

BiliManager::~BiliManager() {
}

void BiliManager::login() {
    if (d_ptr->logged)
        return;
}

void BiliManager::logout() {
    if (!d_ptr->logged)
        return;
}

bool BiliManager::isLogin() const {
    return d_ptr->logged;
}

BiliManager *BiliManager::instance() {
    return self;
}
