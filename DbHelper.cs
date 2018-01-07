using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Data;
using System.Text.RegularExpressions;
using System.Collections;
using System.IO;
using IntegralSystem.Properties;

namespace IntegralSystem
{
    class DbHelper
    {
        private static DbHelper dbHelper;
        SQLiteConnection conn;

        public enum BonusChangeType
        {
            Integral2_1 = 0,
            Integral5_0 = 1,
            Consume = 11
        }

        public enum LogType
        {
            Login = 0,
            LoginError = 1,
            UserUpdate = 11,
            GoodsNew = 20,
            GoodsUpdate = 21,
            GoodsDelete = 22,
            MemberNew = 30,
            MemberUpdate = 31,
            MemberDelete = 32
        }

        public enum BonusChangeStatus
        {
            Normal = 0,
            RevokeMark = 1,
            Revoke = 2
        }

        private DbHelper()
        {
            string dbFile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\IntegralSystem\IntegralSystem.db";
            if (!File.Exists(dbFile))
                dbFile = "IntegralSystem.db";
            conn = new SQLiteConnection("Data Source=" + dbFile);
            conn.SetPassword("SystemIntegral");
            conn.Open();
            //conn.ChangePassword("");
            //conn.Close();
        }

        public static DbHelper Instance
        {
            get
            {
                if (dbHelper == null)
                    dbHelper = new DbHelper();
                return dbHelper;
            }
        }

        void Dispose()
        {
            conn.Close();
        }

        public List<string> GetUsers()
        {
            List<string> users = new List<string>();
            SQLiteCommand cmd = conn.CreateCommand();
            cmd.CommandText = "select id,username,type from user order by lastLoginTime desc";
            SQLiteDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                users.Add(reader.GetString(1));
            }
            reader.Dispose();
            if (users.Count == 0)
            {
                cmd.CommandText = string.Format("insert into user(type,username,password) values(0, '{0}', '{1}')", "admin", CryptoHelper.EncryptAes("admin", CryptoHelper.AesKey));
                cmd.ExecuteNonQuery();
                cmd.CommandText = string.Format("insert into user(type,username,password) values(1, '{0}', '{1}')", "user", CryptoHelper.EncryptAes("user", CryptoHelper.AesKey));
                cmd.ExecuteNonQuery();

                users.Add("admin");
                users.Add("user");
            }
            cmd.Dispose();
            return users;
        }

        public bool UpdateUser(string username, int type, string password)
        {
            List<string> users = new List<string>();
            SQLiteCommand cmd = conn.CreateCommand();
            cmd.CommandText = string.Format("update user set password=@password where username='{0}'", username);
            cmd.Parameters.AddWithValue("@password", CryptoHelper.EncryptAes(password, CryptoHelper.AesKey));
            int result = cmd.ExecuteNonQuery();
            cmd.Dispose();
            return result == 1;
        }

        private void backupDatabase()
        {
            string filename = "";
            if (Settings.Default.LastBackupMonth != DateTime.Now.Month)
            {
                Settings.Default.LastBackupMonth = DateTime.Now.Month;
                Settings.Default.Save();
                filename = Path.GetDirectoryName(conn.FileName) + @"\backup\IntegralSystemM" + DateTime.Now.Month + ".db";
            }
            else if (Settings.Default.LastBackupDay != DateTime.Now.Day)
            {
                Settings.Default.LastBackupDay = DateTime.Now.Day;
                Settings.Default.Save();
                filename = Path.GetDirectoryName(conn.FileName) + @"\backup\IntegralSystemD" + DateTime.Now.Day + ".db";
            }
            if (filename != "" && Directory.Exists(Path.GetDirectoryName(filename)))
            {
                DirectoryInfo di = new DirectoryInfo(Path.GetDirectoryName(filename));
                di.Attributes = FileAttributes.Hidden | di.Attributes;
                File.Copy(conn.FileName, filename, true);
            }
        }

        public int UserLogin(string username, string password)
        {
            List<string> users = new List<string>();
            SQLiteCommand cmd = conn.CreateCommand();
            cmd.CommandText = "select id,type from user where username=@username and password=@password";
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@password", CryptoHelper.EncryptAes(password, CryptoHelper.AesKey));
            SQLiteDataReader reader = cmd.ExecuteReader();
            int type = -1;
            int id = -1;
            if (reader.Read())
            {
                id = reader.GetInt32(0);
                type = reader.GetInt32(1);
                reader.Dispose();
                cmd.CommandText = string.Format("update user set lastLoginTime=datetime('{0}') where id={1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), id);
                cmd.ExecuteNonQuery();
            }
            cmd.Dispose();
            if (type >= 0)
            {
                backupDatabase();
            }
            return type;
        }

        public int GetMaxVipId()
        {
            try
            {
                SQLiteCommand cmd = conn.CreateCommand();
                cmd.CommandText = "select max(vipId) from members";
                object result = cmd.ExecuteScalar();
                cmd.Dispose();
                return int.Parse(result.ToString());
            }
            catch
            {
            }
            return 0;
        }

        public bool InsertVip(int vipId, string vipName, string tel)
        {
            try
            {
                SQLiteCommand cmd = conn.CreateCommand();
                cmd.CommandText = string.Format("insert into members(vipId, vipName, tel) values({0}, '{1}', '{2}')", vipId, vipName, tel);
                int result = cmd.ExecuteNonQuery();
                cmd.Dispose();
                return result == 1;
            }
            catch
            {
            }
            return false;
        }

        public bool UpdateVip(int vipId, string vipName, string tel)
        {
            try
            {
                SQLiteCommand cmd = conn.CreateCommand();
                cmd.CommandText = "update members set vipName=@vipName, tel=@tel where vipId=@vipId";
                cmd.Parameters.AddWithValue("@vipId", vipId);
                cmd.Parameters.AddWithValue("@vipName", vipName);
                cmd.Parameters.AddWithValue("@tel", tel);
                int result = cmd.ExecuteNonQuery();
                cmd.Dispose();
                return result == 1;
            }
            catch
            {
            }
            return false;
        }

        public bool DeleteVip(int vipId)
        {
            try
            {
                SQLiteCommand cmd = conn.CreateCommand();
                //cmd.CommandText = "delete from members where vipId=" + vipId;
                cmd.CommandText = "update members set isDelete=1 where vipId=" + vipId;
                int result = cmd.ExecuteNonQuery();
                cmd.Dispose();
                return result == 1;
            }
            catch
            {
            }
            return false;
        }

        public DataTable GetVipList()
        {
            SQLiteCommand cmd = conn.CreateCommand();
            cmd.CommandText = "select id,vipId,vipName,tel,bonus,maxBonus,playCount,playDuration/60 as playDuration,createTime from members where isDelete<>1";
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            cmd.Dispose();
            return dt;
        }

        public DataTable GetVipList(string text)
        {
            if (text.Length == 0)
                return GetVipList();
            int vipId = 0;
            DataTable all = null;
            DataTable dataTableForVipId = new DataTable();
            DataTable dataTableForTel = new DataTable();
            DataTable dataTableForName = new DataTable();
            if (text.Length == 3 && int.TryParse(text, out vipId) && vipId > 0)
            {
                SQLiteCommand cmd = conn.CreateCommand();
                cmd.CommandText = "select id,vipId,vipName,tel,bonus,createTime,maxBonus from members where isDelete<>1 and vipId=" + vipId;
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);
                adapter.Fill(dataTableForVipId);
                cmd.Dispose();
            }
            if (Regex.IsMatch(text, @"^\d{4,12}$"))
            {
                SQLiteCommand cmd = conn.CreateCommand();
                cmd.CommandText = string.Format("select id,vipId,vipName,tel,bonus,createTime,maxBonus from members where isDelete<>1 and tel like '%{0}%' order by tel", text);
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);
                adapter.Fill(dataTableForTel);
                cmd.Dispose();
            }
            try
            {
                SQLiteCommand cmd = conn.CreateCommand();
                cmd.CommandText = "select id,vipId,vipName,tel,bonus,createTime,maxBonus from members where isDelete<>1 and vipName like @vipName order by vipName";
                cmd.Parameters.AddWithValue("@vipName", "%" + text + "%");
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);
                adapter.Fill(dataTableForName);
                cmd.Dispose();
            }
            catch { }
            if (dataTableForVipId.Columns.Count > 0)
            {
                all = dataTableForVipId;
                dataTableForVipId.PrimaryKey = new DataColumn[] { dataTableForVipId.Columns[0] };
            }
            if (dataTableForTel.Columns.Count > 0)
            {
                if (all == null)
                    all = dataTableForTel;
                dataTableForTel.PrimaryKey = new DataColumn[] { dataTableForTel.Columns[0] };
            }
            if (dataTableForName.Columns.Count > 0)
            {
                if (all == null)
                    all = dataTableForName;
                dataTableForName.PrimaryKey = new DataColumn[] { dataTableForName.Columns[0] };
            }
            if (all != dataTableForTel)
                foreach (DataRow row in dataTableForTel.Rows)
                {
                    if (all.Rows.Contains(row.ItemArray[0]))
                        continue;
                    all.Rows.Add(row.ItemArray);
                }
            if (all != dataTableForName)
                foreach (DataRow row in dataTableForName.Rows)
                {
                    if (all.Rows.Contains(row.ItemArray[0]))
                        continue;
                    all.Rows.Add(row.ItemArray);
                }
            return all;
        }

        public DataTable GetGoodsList()
        {
            return GetGoodsList(true);
        }

        public DataTable GetGoodsList(bool all)
        {
            SQLiteCommand cmd = conn.CreateCommand();
            if (all)
                cmd.CommandText = "select id,name,price,soldCount,soldBonus,createTime from goods where isDelete<>1 order by id";
            else
                cmd.CommandText = "select id,name,price from goods where isDelete<>1 order by soldCount desc,id";
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            cmd.Dispose();
            return dt;
        }

        public bool AddGoods(string name, float price)
        {
            SQLiteCommand cmd = conn.CreateCommand();
            cmd.CommandText = string.Format("insert into goods(name, price) values(@name, {0})", price);
            cmd.Parameters.AddWithValue("@name", name);
            int result = cmd.ExecuteNonQuery();
            cmd.Dispose();
            return result == 1;
        }

        public bool UpdateGoods(int goodsId, string name, float price)
        {
            SQLiteCommand cmd = conn.CreateCommand();
            cmd.CommandText = string.Format("update goods set name=@name,price={0} where id={1}", price, goodsId);
            cmd.Parameters.AddWithValue("@name", name);
            int result = cmd.ExecuteNonQuery();
            cmd.Dispose();
            return result == 1;
        }

        public bool DeleteGoods(int goodsId)
        {
            SQLiteCommand cmd = conn.CreateCommand();
            cmd.CommandText = string.Format("update goods set isDelete=1 where id={0}", goodsId);
            int result = cmd.ExecuteNonQuery();
            cmd.Dispose();
            return result == 1;
        }


        public int IsLastBonusChange(int vipId, List<GoodsInfo> listGoodsInfo, float consumeBonus)
        {
            listGoodsInfo.Sort();
            string consume = "";
            foreach (var goodsInfo in listGoodsInfo)
            {
                consume += string.Format("{0},{1},{2};", goodsInfo.id, goodsInfo.price, goodsInfo.count);
            }
            float changeBonus = -consumeBonus;
            SQLiteCommand cmd = conn.CreateCommand();
            cmd.CommandText = string.Format("select max(createTime) from bonus_change where vipId={0} and status={1} and type={2} and changeBonus={3} and createTime>datetime('{4}') and consume=@consume"
                    , vipId, (int)BonusChangeStatus.Normal, (int)BonusChangeType.Consume, changeBonus, DateTime.Now.AddHours(-1).ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.Parameters.AddWithValue("@consume", consume);
            object obj = cmd.ExecuteScalar();
            cmd.Dispose();
            if (obj is DBNull)
                return -1;
            DateTime lastTime;
            if (DateTime.TryParse(obj.ToString(), out lastTime))
            {
                return (int)(DateTime.Now - lastTime).TotalMinutes;
            }
            return -1;
        }

        public DataTable GetBonusChangeList(int vipId, DateTime startTime)
        {
            SQLiteCommand cmd = conn.CreateCommand();
            if ((DateTime.Now - startTime).TotalMinutes <= 1)
                startTime = DateTime.Now.AddMonths(-1);
            DateTime endTime = startTime.AddMonths(1).AddDays(1);
            if (vipId == 0)
            {
                cmd.CommandText = string.Format("select id,vipId,type,status,case status when 2 then (case type when 0 then '撤销积分（1/2）' when 1 then '撤销积分（5/0）' when 11 then '撤销兑换' else '撤销' end) else (case type when 0 then '积分（1/2）' when 1 then '积分（5/0）' when 11 then '兑换' else '' end) end as typeText,startTime,endTime,duration,changeBonus,currBonus,desc,createTime from bonus_change where createTime>=datetime('{0}') and createTime<datetime('{1}')"
                    , startTime.ToString("yyyy-MM-dd HH:mm:ss"), endTime.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            else
            {
                cmd.CommandText = string.Format("select id,vipId,type,status,case status when 2 then (case type when 0 then '撤销积分（1/2）' when 1 then '撤销积分（5/0）' when 11 then '撤销兑换' else '撤销' end) else (case type when 0 then '积分（1/2）' when 1 then '积分（5/0）' when 11 then '兑换' else '' end) end as typeText,startTime,endTime,duration,changeBonus,currBonus,desc,createTime from bonus_change where vipId={0} and createTime>=datetime('{1}') and createTime<datetime('{2}')"
                    , vipId, startTime.ToString("yyyy-MM-dd HH:mm:ss"), endTime.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            cmd.Dispose();
            return dt;
        }

        class EqualityComparer : IEqualityComparer<VipInfo>
        {
            public bool Equals(VipInfo x, VipInfo y)
            {
                return x.vipId == y.vipId;
            }

            public int GetHashCode(VipInfo obj)
            {
                return obj.vipId;
            }
        }

        public List<VipInfo> FindVips(string val)
        {
            List<VipInfo> list = new List<VipInfo>();
            int intVal = 0;
            VipInfo vipInfoForVipId = null;
            List<VipInfo> listForTel = new List<VipInfo>();
            if (int.TryParse(val, out intVal) && intVal > 0 && intVal < 1000)
            {
                SQLiteCommand cmd = conn.CreateCommand();
                cmd.CommandText = "select vipId,vipName,tel,bonus,maxBonus from members where isDelete<>1 and vipId=" + val;
                SQLiteDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows && reader.Read())
                {
                    vipInfoForVipId = new VipInfo(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetFloat(3), reader.GetFloat(4));
                }
                cmd.Dispose();
            }
            if (Regex.IsMatch(val, @"^\d{4,12}$"))
            {
                SQLiteCommand cmd = conn.CreateCommand();
                cmd.CommandText = string.Format("select vipId,vipName,tel,bonus,maxBonus from members where isDelete<>1 and tel like '%{0}%' order by tel", val);
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    if (vipInfoForVipId != null && vipInfoForVipId.vipId == reader.GetInt32(0))
                        continue;
                    listForTel.Add(new VipInfo(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetFloat(3), reader.GetFloat(4)));
                }
                cmd.Dispose();
            }
            try
            {
                SQLiteCommand cmd = conn.CreateCommand();
                cmd.CommandText = "select vipId,vipName,tel,bonus,maxBonus from members where isDelete<>1 and vipName like @vipName order by vipName";
                cmd.Parameters.AddWithValue("@vipName", "%" + val + "%");
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    if (vipInfoForVipId != null && vipInfoForVipId.vipId == reader.GetInt32(0))
                        continue;
                    VipInfo vipInfo = new VipInfo(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetFloat(3), reader.GetFloat(4));
                    if (!listForTel.Contains(vipInfo, new EqualityComparer()))
                        list.Add(vipInfo);
                }
                cmd.Dispose();
            }
            catch { }
            list.InsertRange(0, listForTel);
            if (vipInfoForVipId != null)
                list.Insert(0, vipInfoForVipId);
            return list;
        }

        public VipInfo GetVipInfo(int vipId)
        {
            VipInfo vipInfo = null;
            SQLiteCommand cmd = conn.CreateCommand();
            cmd.CommandText = "select vipId,vipName,tel,bonus,maxBonus from members where isDelete<>1 and vipId=" + vipId;
            SQLiteDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows && reader.Read())
            {
                vipInfo = new VipInfo(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetFloat(3), reader.GetFloat(4));
            }
            cmd.Dispose();
            return vipInfo;
        }

        public VipInfo GetVipInfoByTel(string val)
        {
            VipInfo vipInfo = null;
            SQLiteCommand cmd = conn.CreateCommand();
            cmd.CommandText = "select vipId,vipName,tel,bonus,maxBonus from members where isDelete<>1 and tel=@tel";
            cmd.Parameters.AddWithValue("@tel", "%" + val + "%");
            SQLiteDataReader reader = cmd.ExecuteReader();
            int count = 0;
            while (reader.Read())
            {
                count++;
                if (count > 1)
                {
                    vipInfo = null;
                    break;
                }
                vipInfo = new VipInfo(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetFloat(3), reader.GetFloat(4));
            }
            cmd.Dispose();
            return vipInfo;
        }

        public bool AddBonus(int vipId, int level, DateTime startTime, DateTime endTime, int duration, float changeBonus, out string msg)
        {
            VipInfo vipInfo = GetVipInfo(vipId);
            if (vipInfo == null)
            {
                msg = "不存在会员号" + vipId;
                return false;
            }
            SQLiteCommand cmd = conn.CreateCommand();
            cmd.CommandText = string.Format("select startTime,endTime from bonus_change where id=(select max(id) from bonus_change where vipId={0} and status={1} and type<{2})"
                , vipId, (int)BonusChangeStatus.Normal, (int)BonusChangeType.Consume);
            SQLiteDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                DateTime lastStartTime = reader.GetDateTime(0);
                DateTime lastEndTime = reader.GetDateTime(1);
                if ((startTime >= lastStartTime && startTime < lastEndTime) || (endTime > lastStartTime && endTime <= lastEndTime))
                {
                    msg = string.Format("当前会员该时间段已经积过分，最后一次积分时段为{0}至{1}", lastStartTime, lastEndTime);
                    cmd.Dispose();
                    return false;
                }
            }
            reader.Dispose();
            SQLiteTransaction transaction = conn.BeginTransaction();
            try
            {
                float currBonus = vipInfo.bonus + changeBonus;
                cmd.CommandText = string.Format("update members set bonus={0},maxBonus=maxBonus+{1},playCount=playCount+1,playDuration=playDuration+{2} where vipId={3}", currBonus, changeBonus, duration, vipId);
                int result = cmd.ExecuteNonQuery();
                if (result != 1)
                {
                    transaction.Rollback();
                    msg = "更新会员积分失败";
                    return false;
                }
                cmd.CommandText = string.Format("insert into bonus_change(vipId, type, startTime, endTime, duration, changeBonus, currBonus) values({0}, {1}, datetime('{2}'), datetime('{3}'), {4}, {5}, {6})"
                    , vipId, level, startTime.ToString("yyyy-MM-dd HH:mm:ss"), endTime.ToString("yyyy-MM-dd HH:mm:ss"), duration, changeBonus, currBonus);
                result = cmd.ExecuteNonQuery();
                if (result != 1)
                {
                    transaction.Rollback();
                    msg = "新增积分记录失败";
                    return false;
                }
                transaction.Commit();
                msg = string.Format("成功积分{0}，当前剩余总积分{1}", changeBonus, currBonus);
                return true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                msg = ex.Message;
                return false;
            }
            finally
            {
                cmd.Dispose();
            }
        }

        public bool ConsumeBonus(int vipId, List<GoodsInfo> listGoodsInfo, float consumeBonus, out string msg)
        {
            VipInfo vipInfo = GetVipInfo(vipId);
            if (vipInfo == null)
            {
                msg = "不存在会员号" + vipId;
                return false;
            }
            if (vipInfo.bonus < consumeBonus || listGoodsInfo.Count == 0)
            {
                msg = "兑换积分失败，会员剩余积分不够兑换商品";
                return false;
            }
            listGoodsInfo.Sort();
            string desc = "兑换：";
            string consume = "";
            foreach (var goodsInfo in listGoodsInfo)
            {
                desc += goodsInfo.ToString() + ",";
                consume += string.Format("{0},{1},{2};", goodsInfo.id, goodsInfo.price, goodsInfo.count);
            }
            desc = desc.TrimEnd(',');
            SQLiteCommand cmd = conn.CreateCommand();
            SQLiteTransaction transaction = conn.BeginTransaction();
            try
            {
                float changeBonus = -consumeBonus;
                float currBonus = vipInfo.bonus + changeBonus;
                cmd.CommandText = string.Format("update members set bonus=bonus-{0} where vipId={1} and isDelete<>1", consumeBonus, vipId);
                int result = cmd.ExecuteNonQuery();
                if (result != 1)
                {
                    transaction.Rollback();
                    msg = "兑换积分失败，更新积分出错";
                    return false;
                }
                cmd.CommandText = string.Format("insert into bonus_change(vipId, type, changeBonus, currBonus, desc,consume) values({0}, {1}, {2}, {3}, @desc, @consume)"
                    , vipId, (int)BonusChangeType.Consume, changeBonus, currBonus);
                cmd.Parameters.AddWithValue("@desc", desc);
                cmd.Parameters.AddWithValue("@consume", consume);
                result = cmd.ExecuteNonQuery();
                if (result != 1)
                {
                    transaction.Rollback();
                    msg = "兑换积分失败，生成兑换记录出错";
                    return false;
                }
                foreach (var goodsInfo in listGoodsInfo)
                {
                    cmd.CommandText = string.Format("update goods set soldCount=soldCount+{0},soldBonus=soldBonus+{1} where id={2}", goodsInfo.count, goodsInfo.price * goodsInfo.count, goodsInfo.id);
                    result = cmd.ExecuteNonQuery();
                    if (result != 1)
                    {
                        transaction.Rollback();
                        msg = "兑换积分失败，更新商品数量出错";
                        return false;
                    }
                }
                transaction.Commit();
                msg = string.Format("积分兑换成功，兑换后会员剩余积分为{0}", currBonus);
                return true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                msg = ex.Message;
                return false;
            }
            finally
            {
                cmd.Dispose();
            }
        }

        public bool RevokeBonus(long id, int type, string typeName, int status, int vipId, float changeBonus, int duration, string consume, out string msg)
        {
            if (status != (int)BonusChangeStatus.Normal)
            {
                msg = "此记录不能撤销";
                return false;
            }
            VipInfo vipInfo = GetVipInfo(vipId);
            if (vipInfo == null)
            {
                msg = "不存在会员号" + vipId;
                return false;
            }
            SQLiteCommand cmd = conn.CreateCommand();
            SQLiteTransaction transaction = conn.BeginTransaction();
            try
            {
                float currBonus = vipInfo.bonus - changeBonus;
                if (type == (int)BonusChangeType.Consume)
                    cmd.CommandText = string.Format("update members set bonus={0} where vipId={2}", currBonus, vipId);
                else
                    cmd.CommandText = string.Format("update members set bonus={0},maxBonus=maxBonus-{1},playCount=playCount-1,playDuration=playDuration-{2} where vipId={3}", currBonus, changeBonus, duration, vipId);
                int result = cmd.ExecuteNonQuery();
                if (result != 1)
                {
                    transaction.Rollback();
                    msg = "更新会员积分失败";
                    return false;
                }
                cmd.CommandText = string.Format("insert into bonus_change(vipId, type, status, changeBonus, currBonus, desc) values({0}, {1}, {2},{3},{4},@desc)"
                    , vipId, type, (int)BonusChangeStatus.Revoke, -changeBonus, currBonus);
                cmd.Parameters.AddWithValue("@desc", "撤销：序号" + id);
                result = cmd.ExecuteNonQuery();
                if (result != 1)
                {
                    transaction.Rollback();
                    msg = string.Format("撤销{0}记录失败", typeName);
                    return false;
                }
                cmd.CommandText = string.Format("update bonus_change set status={0} where id={1} and status={2}", (int)BonusChangeStatus.RevokeMark, id, (int)BonusChangeStatus.Normal);
                result = cmd.ExecuteNonQuery();
                if (result != 1)
                {
                    transaction.Rollback();
                    msg = string.Format("撤销{0}记录失败，可能此记录已撤销过", typeName);
                    return false;
                }
                transaction.Commit();
                msg = string.Format("成功撤销{0}，会员当前总剩余积分为{1}", typeName, currBonus);
                return true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                msg = ex.Message;
                return false;
            }
            finally
            {
                cmd.Dispose();
            }
        }

        public bool InsertLog(LogType type, string msg)
        {
            SQLiteCommand cmd = conn.CreateCommand();
            try
            {
                cmd.CommandText = string.Format("insert into user_log(type,username,desc) values({0}, '{1}', @desc)", (int)type, LoginForm.Username);
                cmd.Parameters.AddWithValue("@desc", msg);
                int result = cmd.ExecuteNonQuery();
                if (result != 1)
                {
                    return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                cmd.Dispose();
            }
        }
    }
}
