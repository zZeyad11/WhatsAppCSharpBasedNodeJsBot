'use strict';

const Constants = require('./util/Constants');

module.exports = {
    Client: require('./Client'),

    version: require('./package.json').version,

    // Structures
    Chat: require('./structures/Chat'),
    PrivateChat: require('./structures/PrivateChat'),
    GroupChat: require('./structures/GroupChat'),
    Message: require('./structures/Message'),
    MessageMedia: require('./structures/MessageMedia'),
    Contact: require('./structures/Contact'),
    PrivateContact: require('./structures/PrivateContact'),
    BusinessContact: require('./structures/BusinessContact'),
    ClientInfo: require('./structures/ClientInfo'),
    Location: require('./structures/Location'),
    ProductMetadata: require('./structures/ProductMetadata'),
    ...Constants
};