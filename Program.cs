// See https://aka.ms/new-console-template for more information

using System.Runtime.Serialization.Formatters.Binary;
using DataBase_BTree;
//Database db = new Database("test.data");
Database db = Database.Create("test.data");
/*
List<int> _keys = new List<int>()
{
    64, 75, 58, 106, 210, 189, 5, 173, 99, 193, 196, 242, 215, 68, 92, 185, 237, 72, 248, 104, 204, 8, 95, 31, 247, 225,
    142, 143, 7, 206, 13, 30, 234, 148, 166, 188, 176, 178, 232, 250, 202, 120, 28, 117, 89, 130, 33, 91, 36, 245, 162,
    145, 203, 153, 190, 214, 62, 236, 139, 77, 85, 133, 199, 208, 161, 55, 97, 86, 134, 14, 80, 187, 115, 163, 50, 191,
    137
};

List<int> values = new List<int>()
{
    302, 572, 558, 664, 854, 740, 578, 981, 452, 764, 419, 904, 899, 936, 281, 348, 596, 610, 605, 675, 1000, 565, 399,
    976, 710, 909, 122, 340, 212, 982, 645, 719, 907, 450, 720, 202, 854, 924, 668, 697, 489, 569, 473, 615, 688, 794,
    326, 832, 315, 556, 694, 112, 781, 164, 684, 706, 440, 172, 117, 170, 305, 611, 133, 240, 861, 942, 367, 162, 630,
    869, 308, 922, 646, 223, 206, 521, 703
};

for (int i = 0; i < 77; i++)
{
    db.AddRecord(_keys[i],values[i]);
}
db.Print();
for (int i = 0; i < 77; i++)
{
    Console.WriteLine();
    Console.WriteLine("------------"+i+"-------------");
    Console.WriteLine("Deleting: "+_keys[i] );
    Console.WriteLine();
    db.Delete(_keys[i]);
    db.Print();
}
*/

db.AddRecord(5,485);

db.AddRecord(3,3441);


db.AddRecord(20,7222);
db.AddRecord(21,4445);

db.AddRecord(22,88);
db.AddRecord(23,73);
db.AddRecord(24,61);
db.AddRecord(25,74);

db.AddRecord(26,14);
db.AddRecord(27,17);
db.AddRecord(12,59);
db.AddRecord(15,121);
db.AddRecord(17,457);
db.AddRecord(19,473);


db.AddRecord(11,111);
db.AddRecord(14,101);
db.AddRecord(16,777);
db.AddRecord(4,234);
db.AddRecord(2,345);
db.AddRecord(1,223);
db.AddRecord(18,7875);

db.AddRecord(9,78);
db.AddRecord(7,45);
db.AddRecord(10,457);
db.AddRecord(6,437);

db.AddRecord(29,595);
db.AddRecord(28,111);


db.AddRecord(30,754);
db.AddRecord(31,65);
db.AddRecord(32,377);
db.AddRecord(33,832);
db.AddRecord(34,555);
db.AddRecord(35,109);


db.AddRecord(36,736);
db.AddRecord(37,7545);
db.AddRecord(38,10789);
db.AddRecord(39,1089);
db.AddRecord(40,753);
db.AddRecord(41,8457);
db.AddRecord(42,73);
db.AddRecord(43,28);
db.AddRecord(44,857);


/*
db.Delete(1);

db.Delete(4);
db.Delete(6);

db.Delete(9);

db.Delete(10);
db.Delete(11);

db.Delete(14);

/*

db.Delete(15);

db.Delete(27);
db.Delete(22);
db.Delete(24);*/
db.Print();
Console.WriteLine("------------------------------");
db.Delete(25);
db.Delete(28);
db.Delete(26);

db.Print();/*
Console.WriteLine("------------------------------");


string inputKey;
string inputData;
inputKey = Console.ReadLine();
while (inputKey != "q")
{
    
    int key = Int32.Parse(inputKey);
    db.Delete(key);
    db.Print();
    Console.WriteLine("------------------------------");
    inputKey = Console.ReadLine();
}


*/
db.Close();
