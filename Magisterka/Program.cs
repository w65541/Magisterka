// See https://aka.ms/new-console-template for more information
using Magisterka.CSV;
using Magisterka.Data;
using Magisterka.Test;
using Parquet.Serialization;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;

Console.WriteLine("Start");
SynchronizationContext.SetSynchronizationContext(null);


DataGenerator generator = new DataGenerator();
Stopwatch stopwatch = Stopwatch.StartNew();

List<BenchmarkResult> csv=new List<BenchmarkResult>();


intTest(100);
stringTest(100);
boolTest(100);
dateTest(100);
doubleTest(100);
mixedTest(100);

void intTest(int x)
{
    List<RecordInt> data = generator.GenerateInt(10000000);
    Testing.runXcsv<RecordInt>(data, "TestResultCsvInt", x);
     Testing.runXcparquet<RecordInt>(data, "TestResultParquetInt", x);
}
void stringTest(int x)
{
    List<RecordString> data = generator.GenerateString(10000000);
    Testing.runXcsv<RecordString>(data, "TestResultCsvString", x);
    Testing.runXcparquet<RecordString>(data, "TestResultParquetString", x);
}

void boolTest(int x)
{
    List<RecordBool> data = generator.GenerateBool(10000000);
    Testing.runXcsv<RecordBool>(data, "TestResultCsvBool", x);
    Testing.runXcparquet<RecordBool>(data, "TestResultParquetBool", x);
}

void dateTest(int x)
{
    List<RecordDate> data = generator.GenerateDate(10000000);
    Testing.runXcsv<RecordDate>(data, "TestResultCsvDate", x);
    Testing.runXcparquet<RecordDate>(data, "TestResultParquetDate", x);
}

void doubleTest(int x)
{
    List<RecordDouble> data = generator.GenerateDouble(10000000);
    Testing.runXcsv<RecordDouble>(data, "TestResultCsvDouble", x);
    Testing.runXcparquet<RecordDouble>(data, "TestResultParquetDouble", x);
}

void mixedTest(int x)
{
    List<RecordMixed> data = generator.GenerateMixed(10000000);
    Testing.runXcsv<RecordMixed>(data, "TestResultCsvMixed", x);
    Testing.runXcparquet<RecordMixed>(data, "TestResultParquetMixed", x);
}