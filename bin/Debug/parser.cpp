#include <stdio.h>
#include <iostream>
#include <string>
#include <fstream>
using namespace std;


int main(){
   string x;
   
   ifstream fin("ignoredExtension.conf");
   ofstream fout("output.txt");
   while (fin >> x){
      bool okay = true;
      for(int i = 0; i < (int) x.size(); i++){
         if (!isupper(x[i]))
            okay = false;
      }
      if (okay){
         fout << x << endl;
      }
   } 
}
