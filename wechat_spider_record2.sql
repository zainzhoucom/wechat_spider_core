/*
 Navicat Premium Data Transfer

 Source Server         : wsl_ubuntu_mysql
 Source Server Type    : MySQL
 Source Server Version : 50729
 Source Host           : localhost:3306
 Source Schema         : wechat_spider_record

 Target Server Type    : MySQL
 Target Server Version : 50729
 File Encoding         : 65001

 Date: 27/06/2020 14:20:34
*/

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

CREATE DATABASE wechat_spider_record CHARACTER SET 'utf8';

USE wechat_spider_record;

-- ----------------------------
-- Table structure for __EFMigrationsHistory
-- ----------------------------
DROP TABLE IF EXISTS `__EFMigrationsHistory`;
CREATE TABLE `__EFMigrationsHistory`  (
  `MigrationId` varchar(150) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `ProductVersion` varchar(32) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  PRIMARY KEY (`MigrationId`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = latin1 COLLATE = latin1_swedish_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for task_start_sign
-- ----------------------------
DROP TABLE IF EXISTS `task_start_sign`;
CREATE TABLE `task_start_sign`  (
  `id` varchar(128) CHARACTER SET latin1 COLLATE latin1_swedish_ci NOT NULL,
  `start_date` text CHARACTER SET latin1 COLLATE latin1_swedish_ci NOT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = latin1 COLLATE = latin1_swedish_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for wechat_account
-- ----------------------------
DROP TABLE IF EXISTS `wechat_account`;
CREATE TABLE `wechat_account`  (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `nick_name` varchar(128) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `alias` varchar(128) CHARACTER SET latin1 COLLATE latin1_swedish_ci NULL DEFAULT NULL,
  `fackid` varchar(128) CHARACTER SET latin1 COLLATE latin1_swedish_ci NULL DEFAULT NULL,
  `round_head_img` varchar(500) CHARACTER SET latin1 COLLATE latin1_swedish_ci NULL DEFAULT NULL,
  `service_type` int(11) NOT NULL,
  `last_update_time` datetime(0) NULL DEFAULT NULL,
  `hometownid` varchar(50) CHARACTER SET latin1 COLLATE latin1_swedish_ci NULL DEFAULT NULL,
  `spider_role` varchar(20) CHARACTER SET latin1 COLLATE latin1_swedish_ci NULL DEFAULT NULL,
  `spider_role1` varchar(20) CHARACTER SET latin1 COLLATE latin1_swedish_ci NULL DEFAULT NULL,
  `spider_role2` varchar(20) CHARACTER SET latin1 COLLATE latin1_swedish_ci NULL DEFAULT NULL,
  `spider_role3` varchar(20) CHARACTER SET latin1 COLLATE latin1_swedish_ci NULL DEFAULT NULL,
  `spider_role4` varchar(20) CHARACTER SET latin1 COLLATE latin1_swedish_ci NULL DEFAULT NULL,
  `spider_role5` varchar(20) CHARACTER SET latin1 COLLATE latin1_swedish_ci NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 7 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for wechat_article
-- ----------------------------
DROP TABLE IF EXISTS `wechat_article`;
CREATE TABLE `wechat_article`  (
  `id` varchar(128) CHARACTER SET latin1 COLLATE latin1_swedish_ci NOT NULL,
  `create_date` datetime(0) NOT NULL,
  `hometownid` varchar(50) CHARACTER SET latin1 COLLATE latin1_swedish_ci NULL DEFAULT NULL,
  `download` bit(1) NOT NULL,
  `local_path` varchar(255) CHARACTER SET latin1 COLLATE latin1_swedish_ci NULL DEFAULT NULL,
  `aid` text CHARACTER SET latin1 COLLATE latin1_swedish_ci NULL,
  `album_id` text CHARACTER SET latin1 COLLATE latin1_swedish_ci NULL,
  `appmsgid` bigint(20) NOT NULL,
  `checking` int(11) NOT NULL,
  `copyright_type` int(11) NOT NULL,
  `cover` text CHARACTER SET latin1 COLLATE latin1_swedish_ci NULL,
  `create_time` datetime(0) NOT NULL,
  `digest` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL,
  `has_red_packet_cover` int(11) NOT NULL,
  `is_original` int(11) NOT NULL,
  `is_pay_subscribe` int(11) NOT NULL,
  `item_show_type` int(11) NOT NULL,
  `itemidx` int(11) NOT NULL,
  `link` text CHARACTER SET latin1 COLLATE latin1_swedish_ci NULL,
  `media_duration` text CHARACTER SET latin1 COLLATE latin1_swedish_ci NULL,
  `mediaapi_publish_status` int(11) NOT NULL,
  `tagid` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL,
  `title` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL,
  `update_time` datetime(0) NOT NULL,
  `account_id` int(11) NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `IX_wechat_article_account_id`(`account_id`) USING BTREE,
  CONSTRAINT `FK_wechat_article_wechat_account_account_id` FOREIGN KEY (`account_id`) REFERENCES `wechat_account` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

SET FOREIGN_KEY_CHECKS = 1;
