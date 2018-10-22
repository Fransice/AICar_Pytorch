import torch
from torch.autograd import Variable
import torch.nn as nn


class Net(nn.Module):
    def __init__(self, shape):  # [2,4,2]
        super(Net, self).__init__()
        self.shape = shape
        self.net = nn.Sequential(  # k快速神经层
            nn.Linear(6, 10),
            nn.ReLU(),
            nn.Linear(10, 10),
            nn.ReLU(),
            nn.Linear(10, 10),
            nn.ReLU(),
            nn.Linear(10, 2)
        )
        self.optimizer = torch.optim.ASGD(self.net.parameters(), lr=0.02)  ##训练

    def NetTest(self, shape):
        for i in range(len(shape) - 1):
            self.net = nn.Sequential(  # k快速神经层
                nn.Linear(shape[i], shape[i + 1]),
                nn.ReLU(),
                nn.Linear(shape[i + 1], shape[i + 1]),
                nn.ReLU(),
                nn.Linear(shape[i + 1], shape[i + 1]),
                nn.ReLU(),
                nn.Linear(shape[i + 1], shape[i + 1]),
                nn.ReLU(),
                nn.Linear(shape[i + 1], shape[i + 1]),
                nn.ReLU(),
                nn.Linear(shape[i + 1], shape[i + 1]),
                nn.ReLU(),
                nn.Linear(shape[i + 1], shape[i + 1]),
                nn.ReLU(),
                nn.Linear(shape[i + 1], shape[i])
            )

    def Learn(self, store):
        loss_func = nn.MSELoss()
        for t in range(1000):  # 训练10次
            x = torch.FloatTensor(store[:, :6])
            y = torch.FloatTensor(store[:, 1:9][:, 1:9][:, 1:9][:, 1:9])
            prediction = self.net(x)
            loss = loss_func(prediction, y)
            self.optimizer.zero_grad()
            loss.backward()
            self.optimizer.step()
            if t % 10 == 0:
                print(loss)
