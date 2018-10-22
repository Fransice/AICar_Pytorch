import torch
from torch.autograd import Variable
import torch.nn as nn
import matplotlib.pyplot as plt
import numpy

torch.manual_seed(1)

# 创建数据
x = torch.unsqueeze(torch.linspace(-1, 1, 100), dim=1)
y = x.pow(2) + 0.2 * torch.rand(x.size())
x, y = Variable(x), Variable(y)
print(x)
print(y)

def save():
    net1 = nn.Sequential(  # k快速神经层
        nn.Linear(1, 10),
        nn.ReLU(),
        nn.Linear(10, 1)
    )
    optimizer = torch.optim.ASGD(net1.parameters(), lr=0.5)  ##训练
    loss_func = nn.MSELoss()

    for t in range(100):  # 训练100次
        prediction = net1(x)
        loss = loss_func(prediction, y)
        optimizer.zero_grad()
        loss.backward()
        optimizer.step()


    torch.save(net1, 'net.pkl')  # 保存数据
    torch.save(net1.state_dict(), 'net_params.pkl')  # 速度更快  保留数据神经元
    # 画图
    plt.figure(1, figsize=(10, 3))
    plt.subplot(131)
    plt.title("Net")
    plt.scatter(x.data.numpy(), y.data.numpy())
    plt.plot(x.data.numpy(), prediction.data.numpy(), 'r-', lw=5)
    plt.show()


def restore_net():  # 第一种提取方法
    net2 = torch.load('net.pkl')
    prediction = net2(x)
    # 画图
    plt.figure(1, figsize=(10, 3))
    plt.subplot(131)
    plt.title("Net2")
    plt.scatter(x.data.numpy(), y.data.numpy())
    plt.plot(x.data.numpy(), prediction.data.numpy(), 'r-', lw=5)


def restore_params():  # 第二种提取方法
    net3 = nn.Sequential(  # 快速神经层
        nn.Linear(1, 10),
        nn.ReLU(),
        nn.Linear(10, 1)
    )
    net3.load_state_dict(torch.load('net_params.pkl'))
    prediction = net3(x)

    # 画图
    plt.figure(1, figsize=(10, 3))
    plt.subplot(131)
    plt.title("Net3")
    plt.scatter(x.data.numpy(), y.data.numpy())
    plt.plot(x.data.numpy(), prediction.data.numpy(), 'r-', lw=5)


save()
restore_net()
restore_params()
