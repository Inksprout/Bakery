# Bakery Project

This code is a simple command line utility program that can be used to calculate the packing breakdown and total cost of a given order.
The program uses two kinds of `JSON` configuation files, an inventory of what is currently available for sale, and an order file.

The path to the inventory file should be passed in as a command line argument when the program starts.
The program will prompt the user for a filename for the order to process.
Orders `JSON` files must be placed in the Orders folder so that the program can easily read them.

This makes it easy to update the inventory if needed, and to process orders.

### How it works

 - The pack breakdown output will have the smallest number of seperate packages possible.
 - When more than one pack breakdown option with the same number of packages is possible, the more expensive option is selected to maximise profit!
 - When the requested number of items cannot fit evenly into the existing packaging sizing, the program will error as these cases need to be manually handled.

### Running the program

1. Open the Project in Visual Studio
2. Build the project in Visual Studio to output the BakeryProject.dll
3. Open a command window and navigate to the location of the BakeryProject.dll `Bakery\bin\Debug\netcoreapp2.2\BakeryProject.dll`
4.  Run Command: ```dotnet BakeryProject.dll ..\..\..\bakeryPriceList.json```
Note: You can use any inventory `JSON` file by passing in a different path here.

4. When prompted enter the file name of a file from the `Orders` folder. No need to include the file path here, as the program will check for a matching file inside `Orders` folder.
5. The output will be printed to the command line, showing the number of different size packs and total cost for each item in the order.

#### Inventory JSON
An Array of products at the Bakery, with their available pack sizes and costs of each.
|	field				|	definition																					|
|	----------------	|	----------------------------------------------------------------------------------------	|
|	name				|	Readable name of the product																|
|	code				|	Unique code for the product																	|
|	packs				|	Array of available packs in the format {"packSize": 8, "price": 22.45}						|

#### Order JSON
An Array of products with the quantity for each one
|	field				|	definition																					|
|	----------------	|	----------------------------------------------------------------------------------------	|
|	code				|	Product code matching one of the codes from the inventory JSON								|
|	quantity			|	The number of the product the order is for.													|

