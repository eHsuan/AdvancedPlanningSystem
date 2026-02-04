# APS 與 MES 系統通訊協議 (HTTP/JSON)

## 1. 概述
本協議定義 Advanced Planning System (APS) 與 Manufacturing Execution System (MES) 之間的通訊標準。
*   **傳輸層**: HTTP 1.1
*   **方法**: POST
*   **資料格式**: JSON (UTF-8)
*   **預設埠號**: 8080 (模擬器/測試環境), 80 (正式環境)

## 2. API 介面定義

### 2.1 搬送請求驗證 (Validate Move)
APS 在決定搬送任務前，需向 MES 詢問該路徑是否合法。

*   **URL**: `/api/validate_move`
*   **Method**: `POST`
*   **Request Body**:
    ```json
    {
      "CassetteID": "CASS-001",
      "SourcePort": "P01",
      "DestinationPort": "EQP-01"
    }
    ```
*   **Response Body**:
    ```json
    {
      "Success": true,
      "Message": "OK",
      "TransactionID": "TRX-20231027-001"
    }
    ```
    *   `Success`: `true` 表示允許搬送，`false` 表示拒絕。

### 2.2 狀態回報 (Report Status)
當 APS 偵測到 Port 狀態改變 (如掃描到 Barcode) 時，回報給 MES。

*   **URL**: `/api/report_status`
*   **Method**: `POST`
*   **Request Body**:
    ```json
    {
      "PortID": "P01",
      "CassetteID": "CASS-001",
      "Status": "Occupied", 
      "EventTime": "2023-10-27T10:00:00"
    }
    ```
    *   `Status`: `Empty`, `Occupied`, `Error`
*   **Response Body**:
    ```json
    {
      "Success": true,
      "Message": "Received"
    }
    ```

### 2.3 取得任務清單 (Get Task List)
APS 定期或主動向 MES 索取當前需執行的緊急任務。

*   **URL**: `/api/get_tasks`
*   **Method**: `POST`
*   **Request Body**:
    ```json
    {
      "EquipmentID": "APS-01"
    }
    ```
*   **Response Body**:
    ```json
    {
      "Tasks": [
        {
          "TaskID": "T001",
          "CassetteID": "CASS-999",
          "From": "P05",
          "To": "EQP-02",
          "Priority": 100
        }
      ]
    }
    ```
