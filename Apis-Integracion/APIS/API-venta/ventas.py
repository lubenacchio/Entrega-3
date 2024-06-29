from flask import Flask, request, jsonify
import requests
import logging
from flask_cors import CORS

app = Flask(__name__)
CORS(app)

CLIENT_API_URL = 'http://localhost:8002/api/cliente/'
SHIPMENT_API_URL = 'http://localhost:8001/estado_envio/'

# Configurar logging
logging.basicConfig(level=logging.INFO)
logger = logging.getLogger(__name__)

@app.route('/generar_boleta/', methods=['POST'])
def generate_receipt():
    data = request.get_json()
    if not data or not data.get('items'):
        logger.error("La compra debe poseer al menos 1 articulo")
        return jsonify({"error": "La compra debe poseer al menos 1 articulo"}), 400

    customer_id = data.get('customer_id')
    if not customer_id:
        logger.error("El customer_id es obligatorio")
        return jsonify({"error": "El customer_id es obligatorio"}), 400

    try:
        total = sum(item['precio'] * item['cantidad'] for item in data['items'])
    except KeyError:
        logger.error("Cada artículo debe tener 'precio' y 'cantidad'")
        return jsonify({"error": "Cada artículo debe tener 'precio' y 'cantidad'"}), 400
    except TypeError:
        logger.error("El 'precio' y 'cantidad' deben ser números")
        return jsonify({"error": "El 'precio' y 'cantidad' deben ser números"}), 400

    receipt = {
        "customer_id": customer_id,
        "items": data['items'],
        "total": total
    }

    # Comunicación con API de Clientes
    try:
        client_response = requests.get(f"{CLIENT_API_URL}/{customer_id}")
        client_response.raise_for_status()
    except requests.RequestException as e:
        logger.error(f"Error al comunicarse con el API de clientes: {str(e)}")
        return jsonify({"error": f"Error al comunicarse con el API de clientes: {str(e)}"}), 500

    order_id = data.get('order_id')
    if not order_id:
        logger.error("El order_id es obligatorio")
        return jsonify({"error": "El order_id es obligatorio"}), 400

    # Comunicación con API de Despacho
    try:
        shipment_response = requests.post(SHIPMENT_API_URL, json={"order_id": order_id, "status": "En proceso", "customer_id": customer_id})
        shipment_response.raise_for_status()
    except requests.RequestException as e:
        logger.error(f"Error al comunicarse con el API de logística: {str(e)}")
        return jsonify({"error": f"Error al comunicarse con el API de logística: {str(e)}"}), 500

    return jsonify(receipt), 200

@app.route('/estado', methods=['GET'])
def status():
    logger.info("Solicitud recibida para verificar el estado de la API")
    return jsonify({"estado": "API de ventas está funcionando"}), 200

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=8000, debug=True)
