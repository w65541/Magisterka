namespace Magisterka.Data
{
    public class DataGenerator
    {
        public List<RecordMixed> GenerateMixed(int x)
        {
            var random = new Random(123);
            var data = new List<RecordMixed>();

            for (int i = 0; i < x; i++)
            {
                data.Add(new RecordMixed
                {
                    Id = i,
                    ValueInt = (int)random.NextInt64(100000),
                    ValueDouble = random.NextDouble() * 1000,
                    Name = "Item_" + i,
                    CreatedDate = DateTime.Now.AddDays(-random.Next(1000)),
                    IsActive = random.Next(0, 2) == 1
                });
            }
            return data;
        }

        public List<RecordInt> GenerateInt(int x)
        {
            var random = new Random(123);
            var data = new List<RecordInt>();

            for (int i = 0; i < x; i++)
            {
                data.Add(new RecordInt
                {
                    Id = i,
                    Value = (int)random.NextInt64(100000)
                });
            }
            return data;
        }

        public List<RecordDate> GenerateDate(int x)
        {
            var random = new Random(123);
            var data = new List<RecordDate>();
            for (int i = 0; i < x; i++)
            {
                data.Add(new RecordDate
                {
                    Id = i,
                    Value = DateTime.Now.AddDays(-random.Next(1000))
                });
            }
            return data;
        }

        public List<RecordString> GenerateString(int x)
        {
            var random = new Random(123);
            var data = new List<RecordString>();
            for (int i = 0; i < x; i++)
            {
                data.Add(new RecordString
                {
                    Id = i,
                    Value = "Item_" + random.Next(100000)
                });
            }
            return data;
        }
        public List<RecordDouble> GenerateDouble(int x)
        {
            var random = new Random(123);
            var data = new List<RecordDouble>();

            for (int i = 0; i < x; i++)
            {
                data.Add(new RecordDouble
                {
                    Id = i,
                    Value = random.NextDouble() * 1000,
                });
            }
            return data;
        }

        public List<RecordBool> GenerateBool(int x) {            
            var random = new Random(123);
            var data = new List<RecordBool>();
            for (int i = 0; i < x; i++)
            {
                data.Add(new RecordBool
                {
                    Id = i,
                    Value = random.Next(0, 2) == 1
                });
            }
            return data;
        }
    }

}
