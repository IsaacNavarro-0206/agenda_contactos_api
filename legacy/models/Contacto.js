import DataTypes from 'sequelize';
import db from '../database/db.js';

export const Contacto = db.define('contactos', {
    id: {
        type: DataTypes.INTEGER,
        primaryKey: true,
        autoIncrement: true
    },
    nombres: {
        type: DataTypes.STRING
    },
    apellidos: {
        type: DataTypes.STRING
    },
    correo: {
        type: DataTypes.STRING
    },
    telefono: {
        type: DataTypes.STRING
    },
    celular: {
        type: DataTypes.STRING
    },
    direccion: {
        type: DataTypes.STRING
    }
});