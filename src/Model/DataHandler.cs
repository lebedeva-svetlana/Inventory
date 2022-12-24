using Microsoft.Data.Sqlite;
using System.Collections.ObjectModel;
using System.Text;

namespace Model
{
    public static class DataHandler
    {
        private static string ConnectionString = "Data Source=inventory.db;Cache=Shared;Mode=ReadWriteCreate;";

        private static SqliteConnection SqlConnection = new(ConnectionString);

        private static string SelectAllQuery = @"SELECT property.property_id AS 'ID имущества', place.place_id AS 'ID помещения', place.name AS 'Помещение', object_type.object_type_id AS 'ID типа имущества', object_type.name AS 'Наименование типа имущество', object.description AS 'Описание', inventory_number AS 'Инвентарный номер', property.in_stock AS 'В наличии' FROM property INNER JOIN place ON property.place_id = place.place_id INNER JOIN object ON property.object_id = object.object_id INNER JOIN object_type ON object.object_type_id = object_type.object_type_id";

        public static void CreateDatabase()
        {
            string query = @"DROP TABLE IF EXISTS property; DROP TABLE IF EXISTS object; DROP TABLE IF EXISTS place; DROP TABLE IF EXISTS object_type; DROP TABLE IF EXISTS user; CREATE TABLE IF NOT EXISTS place ( place_id INTEGER NOT NULL PRIMARY KEY, name TEXT NOT NULL ); CREATE TABLE IF NOT EXISTS object_type ( object_type_id INTEGER NOT NULL PRIMARY KEY, name TEXT NOT NULL ); CREATE TABLE IF NOT EXISTS object ( object_id INTEGER NOT NULL PRIMARY KEY, object_type_id INTEGER, description TEXT, FOREIGN KEY(object_type_id) REFERENCES object_type(object_type_id) ); CREATE TABLE IF NOT EXISTS property ( property_id INTEGER NOT NULL PRIMARY KEY, object_id INTEGER, place_id INTEGET, inventory_number TEXT NOT NULL UNIQUE, in_stock INTEGER(2) NOT NULL DEFAULT 1, FOREIGN KEY(object_id) REFERENCES object(object_id) FOREIGN KEY(place_id) REFERENCES place(place_id) ); CREATE TABLE IF NOT EXISTS user ( user_id INTEGER NOT NULL PRIMARY KEY, username TEXT NOT NULL, password TEXT NOT NULL);";

            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            SqliteCommand command = new(query, connection);

            command.ExecuteNonQuery();
        }

        public static void ExportToCSV(string fileName)
        {
            string query = @"SELECT place.name AS 'Помещение', object_type.name AS 'Наименование типа имущество', object.description AS 'Описание', inventory_number AS 'Инвентарный номер', property.in_stock AS 'В наличии' FROM property INNER JOIN place ON property.place_id = place.place_id INNER JOIN object ON property.object_id = object.object_id INNER JOIN object_type ON object.object_type_id = object_type.object_type_id;";

            SqlConnection.Open();

            SqliteCommand command = new(query, SqlConnection);

            using SqliteDataReader reader = command.ExecuteReader();

            using StreamWriter writer = new(new FileStream(fileName, FileMode.OpenOrCreate), Encoding.UTF8);

            for (int i = 0; i < reader.FieldCount; i++)
            {
                string name = reader.GetName(i);
                if (name.Contains(','))
                {
                    name = "\"" + name + "\"";
                }

                writer.Write(name + ",");
            }

            writer.WriteLine();

            while (reader.Read())
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    string value = (reader.GetValue(i) == DBNull.Value) ? string.Empty : reader.GetValue(i).ToString();

                    if (value.Contains(','))
                    {
                        value = "\"" + value + "\"";
                    }

                    writer.Write(value + ",");
                }
                writer.WriteLine();
            }

            SqlConnection.Close();
        }

        public static void UpdateInStock(long propertyId, bool inStock)
        {
            string query = @"UPDATE property SET in_stock = @inStock WHERE property_id = @propertyId;";

            SqlConnection.Open();

            SqliteCommand command = new(query, SqlConnection);

            int inStockNumb = 1;

            if (!inStock)
            {
                inStockNumb = 0;
            }

            SqliteParameter propertyIdParam = new("@propertyId", propertyId);
            SqliteParameter inStockParam = new("@inStock", inStockNumb);

            command.Parameters.Add(propertyIdParam);
            command.Parameters.Add(inStockParam);

            command.ExecuteNonQuery();

            SqlConnection.Close();
        }

        public static void DeleteProperty(long propertyId)
        {
            string query = @"DELETE FROM property WHERE property_id = @propertyId;";

            SqlConnection.Open();

            SqliteCommand command = new(query, SqlConnection);

            SqliteParameter propertyIdParam = new("@propertyId", propertyId);

            command.Parameters.Add(propertyIdParam);

            command.ExecuteNonQuery();

            SqlConnection.Close();
        }

        public static void InsertObject(long objectTypeId, string description)
        {
            string query = @"INSERT INTO object (object_type_id, description) VALUES (@objectTypeId, @description);";

            SqlConnection.Open();

            SqliteCommand command = new(query, SqlConnection);

            SqliteParameter objectTypeIdParam = new("@objectTypeId", objectTypeId);
            SqliteParameter descriptionParam = new("@description", description);

            command.Parameters.Add(objectTypeIdParam);
            command.Parameters.Add(descriptionParam);

            command.ExecuteNonQuery();

            SqlConnection.Close();
        }

        public static void InsertUser(string username, string password)
        {
            string query = @"INSERT INTO user (username, password) VALUES (@username, @password);";

            SqlConnection.Open();

            SqliteCommand command = new(query, SqlConnection);

            SqliteParameter usernameParam = new("@username", username);
            SqliteParameter passwordParam = new("@password", password);

            command.Parameters.Add(usernameParam);
            command.Parameters.Add(passwordParam);

            command.ExecuteNonQuery();

            SqlConnection.Close();
        }

        public static void InsertObjectType(string objectType)
        {
            string query = @"INSERT INTO object_type (name) VALUES (@objectType);";

            SqlConnection.Open();

            SqliteCommand command = new(query, SqlConnection);

            SqliteParameter objectTypeParam = new("@objectType", objectType);

            command.Parameters.Add(objectTypeParam);

            command.ExecuteNonQuery();

            SqlConnection.Close();
        }

        public static void InsertPlace(string place)
        {
            string query = @"INSERT INTO place (name) VALUES (@place);";

            SqlConnection.Open();

            SqliteCommand command = new(query, SqlConnection);

            SqliteParameter placeParam = new("@place", place);

            command.Parameters.Add(placeParam);

            command.ExecuteNonQuery();

            SqlConnection.Close();
        }

        public static void InsertProperty(long objectId, long placeId, string inventoryNumber)
        {
            string query = @"INSERT INTO property (object_id, place_id, inventory_number) VALUES (@objectId, @placeId, @inventoryNumber);";

            SqlConnection.Open();

            SqliteCommand command = new(query, SqlConnection);

            SqliteParameter objectIdParam = new("@objectId", objectId);
            SqliteParameter placeIdParam = new("@placeId", placeId);
            SqliteParameter inventoryNumberParam = new("@inventoryNumber", inventoryNumber);

            command.Parameters.Add(objectIdParam);
            command.Parameters.Add(placeIdParam);
            command.Parameters.Add(inventoryNumberParam);

            command.ExecuteNonQuery();

            SqlConnection.Close();
        }

        public static void InsertSampleData()
        {
            string query = @"INSERT INTO place (name) VALUES (""Аудитория №306""); INSERT INTO object_type (name) VALUES (""Доска маркерная""), (""Вешалка""), (""Сейф""), (""Стол круглый""), (""Стол""), (""Стул ученический синий""), (""Стул""), (""Экран""), (""Стеллаж""), (""Кресло""), (""Тумба под принтер""), (""Парта ученическая""), (""Шкаф""), (""Принтер""), (""Монитор""), (""Системный блок""), (""Сканер""); INSERT INTO object (object_type_id) VALUES (1), (2), (2), (3), (4), (4), (4), (4), (5), (5), (5), (6), (6), (6), (6), (6), (6), (6), (6), (6), (6), (6), (6), (6), (6), (6), (6), (6), (6), (6), (6), (7), (7), (7), (8), (9), (10), (10), (11), (12), (12), (12), (12), (12), (13), (13), (13), (13), (14), (15), (15), (15), (15), (15), (15), (15), (15), (15), (15), (15), (15), (16), (16), (16), (16), (16), (16), (16), (16), (16), (16), (16), (16), (16), (17); INSERT INTO property (object_id, place_id, inventory_number) VALUES (1, 1, ""101060000000401""), (2, 1, ""101060000000213/1""), (3, 1, ""101060000000213/2""), (4, 1, ""M00000010""), (5, 1, ""M04000470""), (6, 1, ""M04000471""), (7, 1, ""M04000472""), (8, 1, ""M04000473""), (9, 1, ""M04000469""), (10, 1, ""101060000000109/4""), (11, 1, ""10106000000097/5""), (12, 1, ""101060000000428""), (13, 1, ""101060000000451""), (14, 1, ""101060000000446""), (15, 1, ""101060000000437""), (16, 1, ""101060000000445""), (17, 1, ""101060000000440""), (18, 1, ""101060000000431""), (19, 1, ""101060000000420""), (20, 1, ""101060000000429""), (21, 1, ""101060000000425""), (22, 1, ""101060000000443""), (23, 1, ""101060000000424""), (24, 1, ""101060000000450""), (25, 1, ""101060000000423""), (26, 1, ""101060000000430""), (27, 1, ""101060000000436""), (28, 1, ""101060000000426""), (29, 1, ""101060000000434""), (30, 1, ""101060000000422""), (31, 1, ""101060000000438""), (32, 1, ""101060000000137/1""), (33, 1, ""101060000000137/2""), (34, 1, ""101060000000137/3""), (35, 1, ""M000000432""), (36, 1, ""M04000469/""), (37, 1, ""10106000000067""), (38, 1, ""M00000042""), (39, 1, ""101060000000106""), (40, 1, ""10106000000023/21""), (41, 1, ""10106000000023/22""), (42, 1, ""10106000000023/23""), (43, 1, ""10106000000023/24""), (44, 1, ""10106000000023/25""), (45, 1, ""101060000000193""), (46, 1, ""101060000000180""), (47, 1, ""101060000000171""), (48, 1, ""101060000000169""), (49, 1, ""M04000342""), (50, 1, ""000002002006/1""), (51, 1, ""000002002006/2""), (52, 1, ""000002002006/3""), (53, 1, ""000002002006/4""), (54, 1, ""000002002006/5""), (55, 1, ""00000200408""), (56, 1, ""00000200407""), (57, 1, ""M000000378""), (58, 1, ""M000000345""), (59, 1, ""000002001213""), (60, 1, ""M030000373/17""), (61, 1, ""M030000375/17""), (62, 1, ""M04000408""), (63, 1, ""M04000409""), (64, 1, ""M04000410""), (65, 1, ""M04000411""), (66, 1, ""M04000412""), (67, 1, ""M04000413""), (68, 1, ""M04000418""), (69, 1, ""M04000419""), (70, 1, ""M04000415""), (71, 1, ""M04000416""), (72, 1, ""M04000417""), (73, 1, ""M04000379""), (74, 1, ""M03000000378-17""), (75, 1, ""0001360113"");";

            SqlConnection.Open();

            SqliteCommand command = new(query, SqlConnection);

            command.ExecuteNonQuery();

            SqlConnection.Close();
        }

        public static ObservableCollection<Property> SelectAll()
        {
            ObservableCollection<Property> inventory = new();

            SqlConnection.Open();

            SqliteCommand command = new(SelectAllQuery, SqlConnection);

            using SqliteDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    string value = (reader.GetValue(5) == DBNull.Value) ? string.Empty : reader.GetValue(5).ToString();

                    inventory.Add(new(Convert.ToInt64(reader.GetValue(0)), Convert.ToInt64(reader.GetValue(1)), (string)reader.GetValue(2), Convert.ToInt64(reader.GetValue(3)), (string)reader.GetValue(4), value, (string)reader.GetValue(6), reader.GetBoolean(7)));
                }
            }

            SqlConnection.Close();

            return inventory;
        }

        public static ObservableCollection<Property> SelectAllByObjectType(long objectTypeId)
        {
            string query = SelectAllQuery + @" WHERE object_type.object_type_id = @objectTypeId;";

            ObservableCollection<Property> inventory = new();

            SqlConnection.Open();

            SqliteCommand command = new(query, SqlConnection);

            SqliteParameter objectTypeIdParam = new("@objectTypeId", objectTypeId);

            command.Parameters.Add(objectTypeIdParam);

            using SqliteDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    string value = (reader.GetValue(5) == DBNull.Value) ? string.Empty : reader.GetValue(5).ToString();

                    inventory.Add(new(Convert.ToInt64(reader.GetValue(0)), Convert.ToInt64(reader.GetValue(1)), (string)reader.GetValue(2), Convert.ToInt64(reader.GetValue(3)), (string)reader.GetValue(4), value, (string)reader.GetValue(6), reader.GetBoolean(7)));
                }
            }

            SqlConnection.Close();

            return inventory;
        }

        public static ObservableCollection<Property> SelectAllByPlace(long placeId)
        {
            string query = SelectAllQuery + @" WHERE place.place_id = @placeId;";

            ObservableCollection<Property> inventory = new();

            SqlConnection.Open();

            SqliteCommand command = new(query, SqlConnection);

            SqliteParameter placeIdParam = new("@placeId", placeId);

            command.Parameters.Add(placeIdParam);

            using SqliteDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    string value = (reader.GetValue(5) == DBNull.Value) ? string.Empty : reader.GetValue(5).ToString();

                    inventory.Add(new(Convert.ToInt64(reader.GetValue(0)), Convert.ToInt64(reader.GetValue(1)), (string)reader.GetValue(2), Convert.ToInt64(reader.GetValue(3)), (string)reader.GetValue(4), value, (string)reader.GetValue(6), reader.GetBoolean(7)));
                }
            }

            SqlConnection.Close();

            return inventory;
        }

        public static ObservableCollection<Property> SelectAllByPlaceAndObjectType(long placeId, long objectTypeId)
        {
            string query = SelectAllQuery + @" WHERE place.place_id = @placeId AND object_type.object_type_id = @objectTypeId;";

            ObservableCollection<Property> inventory = new();

            SqlConnection.Open();

            SqliteCommand command = new(query, SqlConnection);

            SqliteParameter placeIdParam = new("@placeId", placeId);
            SqliteParameter objectTypeIdParam = new("@objectTypeId", objectTypeId);

            command.Parameters.Add(placeIdParam);
            command.Parameters.Add(objectTypeIdParam);

            using SqliteDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    string value = (reader.GetValue(5) == DBNull.Value) ? string.Empty : reader.GetValue(5).ToString();

                    inventory.Add(new(Convert.ToInt64(reader.GetValue(0)), Convert.ToInt64(reader.GetValue(1)), (string)reader.GetValue(2), Convert.ToInt64(reader.GetValue(3)), (string)reader.GetValue(4), value, (string)reader.GetValue(6), reader.GetBoolean(7)));
                }
            }

            SqlConnection.Close();

            return inventory;
        }

        public static string SelectUserByName(string username)
        {
            string query = @"SELECT password FROM user WHERE username = @username;";

            string hash = "";

            SqlConnection.Open();

            SqliteCommand command = new(query, SqlConnection);

            SqliteParameter usernameParam = new("@username", username);

            command.Parameters.Add(usernameParam);

            using SqliteDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Read();

                hash = reader.GetString(0);
            }

            SqlConnection.Close();

            return hash;
        }

        public static bool HaveUsers()
        {
            string query = @"SELECT user_id FROM user LIMIT 1;";

            SqlConnection.Open();

            SqliteCommand command = new(query, SqlConnection);

            using SqliteDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                if (reader.Read())
                {
                    SqlConnection.Close();
                    return true;
                }
                else
                {
                    SqlConnection.Close();
                    return false;
                }
            }
            else
            {
                SqlConnection.Close();
                return false;
            }
        }

        public static List<string> SelectUsernames()
        {
            string query = @"SELECT username FROM user;";

            List<string> usernames = new();

            SqlConnection.Open();

            SqliteCommand command = new(query, SqlConnection);

            using SqliteDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    usernames.Add(reader.GetString(0));
                }
            }

            SqlConnection.Close();

            return usernames;
        }

        public static ObservableCollection<Object> SelectObjects()
        {
            string query = @"SELECT object_id, object.object_type_id, object_type.name, description FROM object INNER JOIN object_type ON object.object_type_id = object_type.object_type_id;";

            ObservableCollection<Object> objects = new();

            SqlConnection.Open();

            SqliteCommand command = new(query, SqlConnection);

            using SqliteDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    string value = (reader.GetValue(3) == DBNull.Value) ? string.Empty : reader.GetValue(3).ToString();

                    objects.Add(new(Convert.ToInt64(reader.GetValue(0)), new ObjectType(Convert.ToInt64(reader.GetValue(1)), (string)reader.GetValue(2)), value));
                }
            }

            SqlConnection.Close();

            return objects;
        }

        public static ObjectType SelectObjectType(string objectTypeName)
        {
            string query = @"SELECT object_type_id, name FROM object_type WHERE name = @objectTypeName;";

            SqlConnection.Open();

            SqliteCommand command = new(query, SqlConnection);

            SqliteParameter objectTypeNameParam = new("@objectTypeName", objectTypeName);

            command.Parameters.Add(objectTypeNameParam);

            using SqliteDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Read();

                ObjectType objectType = new(Convert.ToInt64(reader.GetValue(0)), (string)reader.GetValue(1));

                SqlConnection.Close();

                return objectType;
            }
            else
            {
                SqlConnection.Close();

                return null;
            }
        }

        public static ObservableCollection<ObjectType> SelectObjectTypes()
        {
            string query = @"SELECT object_type_id, name FROM object_type;";

            ObservableCollection<ObjectType> objectTypes = new();

            SqlConnection.Open();

            SqliteCommand command = new(query, SqlConnection);

            using SqliteDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    objectTypes.Add(new(Convert.ToInt64(reader.GetValue(0)), (string)reader.GetValue(1)));
                }
            }

            SqlConnection.Close();

            return objectTypes;
        }

        public static Place SelectPlace(string placeName)
        {
            string query = @"SELECT place_id, name FROM place WHERE name = @placeName;";

            SqlConnection.Open();

            SqliteCommand command = new(query, SqlConnection);

            SqliteParameter placeNameParam = new("@placeName", placeName);

            command.Parameters.Add(placeNameParam);

            using SqliteDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Read();

                Place place = new(Convert.ToInt64(reader.GetValue(0)), (string)reader.GetValue(1));

                SqlConnection.Close();

                return place;
            }
            else
            {
                SqlConnection.Close();
                return null;
            }
        }

        public static ObservableCollection<Place> SelectPlaces()
        {
            string query = @"SELECT place_id, name FROM place;";

            ObservableCollection<Place> places = new();

            SqlConnection.Open();

            SqliteCommand command = new(query, SqlConnection);

            using SqliteDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    places.Add(new(Convert.ToInt64(reader.GetValue(0)), (string)reader.GetValue(1)));
                }
            }

            SqlConnection.Close();

            return places;
        }

        public static Property SelectProperty(string inventoryNumber)
        {
            string query = SelectAllQuery + " WHERE inventory_number = @inventoryNumber;";

            SqlConnection.Open();

            SqliteCommand command = new(query, SqlConnection);

            SqliteParameter inventoryNumberParam = new("@inventoryNumber", inventoryNumber);

            command.Parameters.Add(inventoryNumberParam);

            using SqliteDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Read();

                string value = (reader.GetValue(5) == DBNull.Value) ? string.Empty : reader.GetValue(5).ToString();

                Property property = new(Convert.ToInt64(reader.GetValue(0)), Convert.ToInt64(reader.GetValue(1)), (string)reader.GetValue(2), Convert.ToInt64(reader.GetValue(3)), (string)reader.GetValue(4), value, (string)reader.GetValue(6), reader.GetBoolean(7));

                SqlConnection.Close();

                return property;
            }
            else
            {
                SqlConnection.Close();

                return null;
            }
        }
    }
}