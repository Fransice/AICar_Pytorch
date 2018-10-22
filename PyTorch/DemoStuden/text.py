import torch
import torch.nn as nn

net = nn.Sequential(
    torch.nn.Linear(2, 10),  # 神经第一层
    torch.nn.ReLU(),  # 激励函数
    torch.nn.Linear(10, 2)  # 神经第二层
)
print(net)
