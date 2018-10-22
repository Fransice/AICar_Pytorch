import MyNet
import MyTool

p2u = MyTool.Py2Unity()
net = MyNet.Net([6,12,2])
ws = MyTool.get_net_weights(net)
MyTool.save_weights(ws,"C:\\Users\\13290\\Desktop\\weights.txt")
p2u.SendToUnity("save_ok")
while 1:
    p2u.RecFromUnity()
    store = MyTool.read_store("C:\\Users\\13290\\Desktop\\store.txt")
    net.Learn(store)
    print(net)
    ws = MyTool.get_net_weights(net)
    MyTool.save_weights(ws, "C:\\Users\\13290\\Desktop\\weights.txt")
    p2u.SendToUnity("save_ok")