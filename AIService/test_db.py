
import mysql.connector
from config import MYSQL

try:
    connection = mysql.connector.connect(**MYSQL)

    if connection.is_connected():
        print("SUCCESS: MySQL connected!")

        cursor = connection.cursor()
        cursor.execute("SELECT DATABASE()")
        result = cursor.fetchone()

        print("Connected Database:", result[0])

        cursor.close()
        connection.close()

except mysql.connector.Error as error:
    print("MySQL Connection Failed:", error)
