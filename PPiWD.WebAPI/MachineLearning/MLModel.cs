using System.Globalization;
using PPiWD.WebAPI.Models.Measurements;
using Python.Runtime;

namespace PPiWD.WebAPI.MachineLearning;

public class MLModel
{
    public MLModel()
    {
        PythonEngine.Initialize();
    }

    public int Calculate(Measurement measurement)
    {
        Environment.SetEnvironmentVariable("PYTHONNET_PYDLL", @"C:\Python310\python310.dll");

        var code = this.PyCode;
        var data = this.ReplaceFirst(code, "data_mock", this.BuildFromMeasurement(measurement));
        
        using (Py.GIL())
        {
            using (var scope = Py.CreateScope())
            {
                scope.Exec(data);
                var returned = scope.Get<int>("count");
                return returned;
            }
        }
    }

    private string BuildFromMeasurement(Measurement measurement)
    {
        var data = measurement.SensorDatas.Select(sensorData =>
                $"[{sensorData.XAxis.ToString(CultureInfo.InvariantCulture)}, {sensorData.YAxis.ToString(CultureInfo.InvariantCulture)}, {sensorData.ZAxis.ToString(CultureInfo.InvariantCulture)}]")
            .ToList();

        return $"data_mock = [{string.Join(",", data)}]";
    }

    private string ReplaceFirst(string text, string search, string replace)
    {
        var pos = text.IndexOf(search);
        if (pos < 0)
        {
            return text;
        }

        return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
    }

    private readonly string PyCode = @"
import joblib
import numpy as np

WINDOW_SIZE = 200
REPETITION_DURATION = 150
MODEL_PATH = 'model.joblib'

model = joblib.load(MODEL_PATH)

def count_repetitions(x_data):
    x_windowed = []
    for i in range(len(x_data) - WINDOW_SIZE + 1):
        window = x_data[i : i + WINDOW_SIZE]
        x_windowed.append(window)    
    x_windowed = np.array(x_windowed)
    x_windowed = x_windowed.reshape(len(x_windowed), -1)

    preds = model.predict(x_windowed)

    preds_blurred = np.copy(preds)
    for i in range(2, len(preds_blurred)-3):
        if preds[i] == 1:
            preds_blurred[i-2:i+3] = [1,1,1,1,1]

    count = 0
    in_sequence = False
    for num in preds_blurred:
        if num == 1:
            if not in_sequence:
                in_sequence = True
                count += 1
        else:
            in_sequence = False
    print(count)
    return count
    
data_mock

count=count_repetitions(data_mock)";
}