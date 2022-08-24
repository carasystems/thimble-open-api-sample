package sign;

import static java.nio.charset.StandardCharsets.*;
import java.security.InvalidKeyException;
import java.security.NoSuchAlgorithmException;
import javax.crypto.Mac;
import javax.crypto.spec.SecretKeySpec;

public class Main {
  private static final char[] HEX_ARRAY = "0123456789abcdef".toCharArray();
  public static String bytesToHex(byte[] bytes) {
      char[] hexChars = new char[bytes.length * 2];
      for (int j = 0; j < bytes.length; j++) {
          int v = bytes[j] & 0xFF;
          hexChars[j * 2] = HEX_ARRAY[v >>> 4];
          hexChars[j * 2 + 1] = HEX_ARRAY[v & 0x0F];
      }
      return new String(hexChars);
  }

  private static String sign(String appKey, String secret, String timestamp, String url, String reqData) throws NoSuchAlgorithmException, InvalidKeyException {
    System.out.println("reqData: " + reqData);

    Mac mac = Mac.getInstance("HmacSHA256");
    SecretKeySpec secretKeySpec = new SecretKeySpec(secret.getBytes(UTF_8),"HmacSHA256");
    mac.init(secretKeySpec);

    String data = appKey + url + timestamp + reqData;
    byte[] bytes = mac.doFinal(data.getBytes(UTF_8));

    return bytesToHex(bytes);
  }

  public static void main(String[] args) throws NoSuchAlgorithmException, InvalidKeyException {
    String appKey = "<Your-App-Key-Here>"; // The appkey thimble provided
    String secret = "<Your-App-Secret-Here>"; // The secret thimble provided
    String timestamp = "1661312010"; // Epoch Unix Time Stamp
    String url = "/insurance-plans"; // The request url, contains query string. Say a request to "https://open-api.thimble.com/activities?q=1", the 'url' would be /activities?q=1.
    String reqData = "{\"zipcode\":\"07666\",\"activity_id\":\"ACT-00030\"}"; // The request data, convert it into a string

    // POST
    System.out.println("hash: " + sign(appKey, secret, timestamp, url, reqData));

    // GET
    System.out.println("hash: " + sign(appKey, secret, timestamp, url, ""));
  }
}
