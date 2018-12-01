using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;

public class Crypto
{
    SymmetricAlgorithm mCSP;
    #region "Constants"
    private object _key = "12345678";
    #endregion
    public Crypto()
    {

    }

    public bool HasKey()
    {
        return (!(_key == null || _key.ToString() == ""));
    }
    private string SetLengthString(string str, int length)
    {
        while (length > str.Length)
        {
            str += str;
        }
        if (str.Length > length)
        {
            str = str.Remove(length);
        }
        return str;
    }
    public string EncryptString(string Value)
    {
        mCSP = SetEnc();
        string iv = "PenS8UCVF7s=";
        mCSP.IV = Convert.FromBase64String(iv);
        string key = SetLengthString(_key.ToString(), 32);
        mCSP.Key = Convert.FromBase64String(key);
        ICryptoTransform ct;
        MemoryStream ms;
        CryptoStream cs;
        Byte[] byt = new byte[64];

        try
        {
            ct = mCSP.CreateEncryptor(mCSP.Key, mCSP.IV);

            byt = Encoding.UTF8.GetBytes(Value);

            ms = new MemoryStream();
            cs = new CryptoStream(ms, ct, CryptoStreamMode.Write);
            cs.Write(byt, 0, byt.Length);
            cs.FlushFinalBlock();

            cs.Close();

            return Convert.ToBase64String(ms.ToArray());
        }
        catch (Exception ex)
        {
            throw (new Exception("An error occurred while encrypting string " + ex));
        }
    }

    public string DecryptString(string Value)
    {
        mCSP = SetEnc();
        string iv = "PenS8UCVF7s=";
        mCSP.IV = Convert.FromBase64String(iv);
        string key = SetLengthString(_key.ToString(), 32);
        mCSP.Key = Convert.FromBase64String(key);
        ICryptoTransform ct;
        MemoryStream ms;
        CryptoStream cs;
        Byte[] byt = new byte[64];
        try
        {
            ct = mCSP.CreateDecryptor(mCSP.Key, mCSP.IV);

            byt = Convert.FromBase64String(Value);

            ms = new MemoryStream();
            cs = new CryptoStream(ms, ct, CryptoStreamMode.Write);
            cs.Write(byt, 0, byt.Length);
            cs.FlushFinalBlock();

            cs.Close();

            string str = Encoding.UTF8.GetString(ms.ToArray());
            return str;
        }
        catch (Exception ex)
        {
            throw (new Exception("An error occurred while decrypting string " + ex));
        }
    }

    private SymmetricAlgorithm SetEnc()
    {
        return new TripleDESCryptoServiceProvider();
    }
}