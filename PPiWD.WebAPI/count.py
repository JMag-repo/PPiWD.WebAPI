import joblib
import pandas as pd
import numpy as np

WINDOW_SIZE = 200
REPETITION_DURATION = 150
MODEL_PATH = 'model.joblib'
DATA_CSV_PATH = 'data.csv'

model = joblib.load(MODEL_PATH)

x_data = df[['PITCH', 'ROLL', 'YAW']].values

x_windowed = []
for i in range(len(x_data) - WINDOW_SIZE + 1):
    window = x_data[i : i + WINDOW_SIZE]
    x_windowed.append(window)
    
x_windowed = np.array(x_windowed)
x_windowed = x_windowed.reshape(len(x_windowed), -1)

preds = model.predict(x_windowed)

    vec_blurred = np.copy(vec)
    
    for i in range(2, len(vec)-3):
        if vec[i] == 1:
            vec_blurred[i-2:i+3] = [1,1,1,1,1]
    import joblib
import pandas as pd
import numpy as np

WINDOW_SIZE = 200
REPETITION_DURATION = 150
MODEL_PATH = 'model.joblib'
DATA_CSV_PATH = 'data.csv'

model = joblib.load(MODEL_PATH)

df = pd.read_csv(DATA_CSV_PATH, sep=';')
x_data = df[['PITCH', 'ROLL', 'YAW']].values
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
    count = 0
    in_sequence = False

    for num in vec_blurred:
        if num == 1:
            if not in_sequence:
                in_sequence = True
                count += 1
        else:
            in_sequence = False
